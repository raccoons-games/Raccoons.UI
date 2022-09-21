using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Raccoons.UI.Guids;

namespace Raccoons.UI.Screens
{
    public abstract class BaseScreenManager : BaseScreenManager<BaseScreen> { }
    public abstract class BaseScreenManager<T> : MonoBehaviour where T : BaseScreen
    {
        [SerializeField]
        private List<T> _allScreens;

        [Header("ReadOnly")]
        [SerializeField]
        private List<T> _allOpenedScreens;

        public event EventHandler<IScreen> OnOpenedScreen;
        public event EventHandler<IScreen> OnClosedScreen;

        protected List<T> AllOpenedScreens { get => _allOpenedScreens; }
        protected List<T> AllScreens { get => _allScreens; }

        #region Awake
        protected virtual void Awake()
        {
            _allScreens.ForEach(
                screen =>
                {
                    screen.OnOpened += Screen_OnOpened;
                    screen.OnClosed += Screen_OnClosed;
                });
            _allOpenedScreens = new List<T>();
            _allOpenedScreens = _allScreens.FindAll(x => x.IsOpen() == true);
        }
        #endregion

        #region Open / Close
        protected virtual void ShowScreen(IScreen screen)
        {
            if (screen != null)
            {
                screen.Open();
            }
            else
            {
                Debug.LogError($"{gameObject.name} Screen == null!");
            }
        }
        public virtual void ShowScreen<R>() where R: BaseScreen
        {
            R screen = GetScreen<R>(typeof(R));
            ShowScreen(screen);
        }
        protected virtual void CloseScreen(IScreen screen)
        {
            if (screen != null)
            {
                screen.Close();
            }
        }

        public virtual void CloseAllScreens()
        {
            List<BaseScreen> temp = new List<BaseScreen>(_allOpenedScreens);
            temp?.ForEach(screen => CloseScreen(screen));
        }
        #endregion

        #region Opened screens
        private void AddOpenedScreen(IScreen screen)
        {
            T baseScreen = screen as T;
            if (screen != null)
            {
                if (_allOpenedScreens.Contains(baseScreen) == false)
                {
                    _allOpenedScreens.Add(baseScreen);
                }
            }
        }
        private void RemoveOpenedScreen(IScreen screen)
        {
            T baseScreen = screen as T;
            if (screen != null)
            {
                if (_allOpenedScreens.Contains(baseScreen) == true)
                {
                    _allOpenedScreens.Remove(baseScreen);
                }
            }
        }
        #endregion

        #region Get
        protected R GetScreen<R>(Type type) where R : BaseScreen
        {
            R result = _allScreens.Find(x => x.GetType() == type) as R;
         //   R result = allScreens.Find(x => x.GetType() == typeof(R)) as R;
            return result;
        }
        protected R GetScreen<R>(string key) where R : BaseScreen
        {
            List<T> screens = _allScreens.FindAll(x => x.GetType() is R);
            R result = screens.Find(x => x.Key == key) as R;
            return result;
        }
        protected R GetScreen<R>() where R : BaseScreen
        {
            return GetScreen<R>(typeof(R));
        }
        protected R GetScreen<R>(IUIGuid guid) where R : BaseScreen
        {
            R result = null;
            if (guid != null)
            {
                List<T> screens = _allScreens.FindAll(x => x.Guid == guid);
                result = screens.Find(x => x.GetType() == typeof(R)) as R;
            }
            return result;
        }

        #endregion

            #region Events
        protected virtual void Screen_OnClosed(object sender, IScreen e)
        {
            RemoveOpenedScreen(e);
            OnClosedScreen?.Invoke(this, e);
        }

        protected virtual void Screen_OnOpened(object sender, IScreen e)
        {
            AddOpenedScreen(e);
            OnOpenedScreen?.Invoke(this, e);
        }
        #endregion

        #region OnDestroy
        protected virtual void OnDestroy()
        {
            _allScreens.ForEach(
               screen =>
               {
                   screen.OnOpened -= Screen_OnOpened;
                   screen.OnClosed -= Screen_OnClosed;
               });
        }
        #endregion
    }
}