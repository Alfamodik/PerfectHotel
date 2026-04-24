using System.Collections.Generic;
using Game.Level.Cash;
using Game.Level;
using Injection;
using UnityEngine;
using Game.Core;
using Game.Config;
using Game.Level.Item;
using Game.Level.Entity;

namespace Game.Modules.CashModule
{
    public sealed class CashModule : Module<CashModuleView>
    {
        private const float _heightAbovePile = 3f;
        private const float _heightAbovePlayer = 1.5f;
        private const float _cashFlyToRemoveRate = 0.1f;
        private const float _checkPlayerOnItemRate = 0.1f;
        private const string _takeSoundName = "take-sound";
        private const float _takeSoundStopDelay = 0.2f;
        private const int _initialCashPacksPerCollect = 1;
        private const int _maxCashPacksPerCollect = 20;
        private const float _cashCollectAccelerationStepTime = 0.1f;

        [Inject] private GameManager _gameManager;
        [Inject] private Context _context;
        [Inject] private Timer _timer;
        [Inject] private GameConfig _config;


        private readonly Dictionary<CashPileView, CashPileController> _cashPilesMap;
        private readonly Dictionary<ItemController, CashPileView> _itemsMap;
        private readonly Dictionary<ItemController, float> _cashCollectStartTimes;
        private readonly HashSet<CashController> _takeSoundCashes;
        private List<CashController> _tempCashes;

        private float _cashFlyToRemoveTimer;
        private float _checkPlayerOnItemTime;
        private float _cashPileRadius;
        private float _takeSoundLastActivityTime;
        private bool _takeSoundPlaying;

        public CashModule(CashModuleView view) : base(view)
        {
            _cashPilesMap = new Dictionary<CashPileView, CashPileController>();
            _itemsMap = new Dictionary<ItemController, CashPileView>();
            _cashCollectStartTimes = new Dictionary<ItemController, float>();
            _takeSoundCashes = new HashSet<CashController>();
            _tempCashes = new List<CashController>();
        }

        public override void Initialize()
        {
            _cashPileRadius = _config.CashPileRadius;

            AddCashPile(_gameManager.Reception.View.CashPileView, _gameManager.Reception.ItemCashPile, _gameManager.Reception.Model);

            foreach (var room in _gameManager.Rooms)
            {
                AddCashPile(room.View.CashPileView, room.ItemCashPile, room.Model);
            }

            foreach (var toilet in _gameManager.Toilets)
            {
                AddCashPile(toilet.View.CashPileView, toilet.ItemCashPile, toilet.Model);
            }

            _gameManager.FLY_TO_REMOVE_CASH += CashFlyToRemove;
            _timer.TICK += OnTick;
        }

        public override void Dispose()
        {
            _gameManager.FLY_TO_REMOVE_CASH -= CashFlyToRemove;
            _timer.TICK -= OnTick;

            foreach (var cashPile in _cashPilesMap.Values)
            {
                cashPile.View.CASH_FLY_TO_PILE -= CashFlyToPile;
                cashPile.View.CASH_FLY_TO_PLAYER -= CashFlyToPlayer;

                foreach (var cash in cashPile.View.Cashes)
                {
                    cash.REMOVE_CASH -= OnRemoveCash;
                    cash.Dispose();
                }
                cashPile.View.Cashes.Clear();
            }

            foreach (var cash in _tempCashes)
            {
                cash.REMOVE_CASH -= OnRemoveCash;
                cash.Dispose();
            }
            _tempCashes.Clear();
            _cashCollectStartTimes.Clear();
            _takeSoundCashes.Clear();
            StopTakeSound();

            _view.ReleaseAllInstances();
        }

        private void OnTick()
        {
            if (Time.time >= _checkPlayerOnItemTime)
            {
                _checkPlayerOnItemTime = Time.time + _checkPlayerOnItemRate;

                foreach (var item in _itemsMap.Keys)
                {
                    float distance = Vector3.Distance(item.Transform.position, _gameManager.Player.View.Position);
                    if (distance < _cashPileRadius)
                    {
                        PlayerOnItem(item);
                    }
                    else
                    {
                        _cashCollectStartTimes.Remove(item);
                    }
                }
            }

            TryStopTakeSound();
        }

        private void AddCashPile(CashPileView view, ItemController itemCashPile, EntityModel model)
        {
            view.CASH_FLY_TO_PILE += CashFlyToPile;
            view.CASH_FLY_TO_PLAYER += CashFlyToPlayer;

            var pile = new CashPileController(view, model);

            _cashPilesMap[view] = pile;
            _itemsMap[itemCashPile] = view;

            _gameManager.AddItem(itemCashPile);
        }

        private CashController Cash(Vector3 position)
        {
            var cashView = _view.Get<CashView>();
            var cash = new CashController(cashView, position, _context);
            return cash;
        }

        private void CashFlyToPile(CashPileView view, Vector3 endPosition)
        {
            CashController cash = Cash(view.transform.position + (Vector3.up * _heightAbovePile));
            cash.FlyToPile(endPosition);
            view.Cashes.Add(cash);
            cash.REMOVE_CASH += OnRemoveCash;
        }

        private void CashFlyToPlayer(CashPileView cashPileView, int index)
        {
            var cash = cashPileView.Cashes[index];
            cash.FlyToPlayer();
            cashPileView.Cashes.Remove(cash);
            _tempCashes.Add(cash);
            RegisterTakeSoundCash(cash);
        }

        private void CashFlyToRemove(Vector3 endPosition)
        {
            _cashFlyToRemoveTimer += Time.deltaTime;
            if (_cashFlyToRemoveTimer < _cashFlyToRemoveRate) return;

            _cashFlyToRemoveTimer = 0f;

            CashController cash = Cash(_gameManager.Player.View.transform.position + (Vector3.up * _heightAbovePlayer));
            cash.FlyToRemove(endPosition);
            cash.REMOVE_CASH += OnRemoveCash;
            RegisterTakeSoundCash(cash);
        }

        private void OnRemoveCash(CashController cash)
        {
            cash.REMOVE_CASH -= OnRemoveCash;
            _takeSoundCashes.Remove(cash);
            _view.Release(cash.View);
            cash.Dispose();
            _tempCashes.Remove(cash);
        }

        private void PlayerOnItem(ItemController item)
        {
            var cashPileView = _itemsMap[item];
            var cashPile = _cashPilesMap[cashPileView];

            if (cashPile.Model.Cash <= 0) return;

            int packsToCollect = GetCashPacksPerCollect(item);
            long amount = 0;

            for (int i = 0; i < packsToCollect; i++)
            {
                long remainingCash = cashPile.Model.Cash - amount;
                if (remainingCash <= 0)
                    break;

                cashPileView.TryFlyCashToPlayer();
                amount += remainingCash < CashPileView.DollarsPerPack
                    ? remainingCash
                    : CashPileView.DollarsPerPack;
            }

            if (amount <= 0) return;

            cashPile.Model.Cash -= amount;
            _gameManager.Model.SavePlaceCash(cashPile.Model.ID, cashPile.Model.Cash);
            cashPile.Model.SetChanged();

            _gameManager.Model.AddCash(amount);
            StartTakeSound();

            if (cashPile.Model.Cash <= 0)
                _cashCollectStartTimes.Remove(item);
        }

        private int GetCashPacksPerCollect(ItemController item)
        {
            if (!_cashCollectStartTimes.ContainsKey(item))
                _cashCollectStartTimes[item] = Time.time;

            float collectDuration = Time.time - _cashCollectStartTimes[item];
            int accelerationSteps = Mathf.FloorToInt(collectDuration / _cashCollectAccelerationStepTime);
            int packsToCollect = _initialCashPacksPerCollect + accelerationSteps;

            return Mathf.Clamp(packsToCollect, _initialCashPacksPerCollect, _maxCashPacksPerCollect);
        }

        private void RegisterTakeSoundCash(CashController cash)
        {
            _takeSoundCashes.Add(cash);
            StartTakeSound();
        }

        private void StartTakeSound()
        {
            _takeSoundLastActivityTime = Time.time;

            if (_takeSoundPlaying)
                return;

            _takeSoundPlaying = true;
            SFXProvider.PlayOnce(_takeSoundName);
        }

        private void TryStopTakeSound()
        {
            if (!_takeSoundPlaying)
                return;

            if (_takeSoundCashes.Count > 0)
                return;

            if (Time.time - _takeSoundLastActivityTime < _takeSoundStopDelay)
                return;

            StopTakeSound();
        }

        private void StopTakeSound()
        {
            if (!_takeSoundPlaying)
                return;

            _takeSoundPlaying = false;
            SFXProvider.Stop(_takeSoundName);
        }
    }
}
