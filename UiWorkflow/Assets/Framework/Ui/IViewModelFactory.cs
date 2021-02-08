using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace Framework.Ui
{
    public interface IViewModelFactory
    {
        TViewModel Build<TViewModel>(string optName) where TViewModel : ViewModelMonoBehaviour;
        void Free(ViewModelMonoBehaviour viewModel);
    }

    class PrefabsViewModelFactory : IViewModelFactory
    {
        private readonly Dictionary<Type, ViewModelMonoBehaviour> _registeredPrefabs =
            new Dictionary<Type, ViewModelMonoBehaviour>();

        public void Register<TViewModel>(TViewModel model, string optName = null)
            where TViewModel : ViewModelMonoBehaviour
        {
            Assert.IsTrue(optName.IsNullOrEmpty(), "OptionalName for ViewModels is not implemented");
            _registeredPrefabs[model.GetType()] = model;
        }

        public TViewModel Build<TViewModel>(string optName) where TViewModel : ViewModelMonoBehaviour
        {
            Assert.IsTrue(optName.IsNullOrEmpty(), "OptionalName for ViewModels is not implemented");
            if (_registeredPrefabs.TryGetValue(typeof(TViewModel), out var viewModelPrefab))
                return Object.Instantiate((TViewModel) viewModelPrefab);
            return default;
        }

        public void Free(ViewModelMonoBehaviour viewModel)
        {
            //TODO: create strategies for handling creating and destroying viewModels
            Object.Destroy(viewModel);
        }
    }

    class InstancesViewModelFactory : IViewModelFactory
    {
        private readonly Dictionary<Type, ViewModelMonoBehaviour> _registeredPrefabs =
            new Dictionary<Type, ViewModelMonoBehaviour>();

        public TViewModel Register<TViewModel>(TViewModel model, string optName = null)
            where TViewModel : ViewModelMonoBehaviour
        {
            Assert.IsTrue(optName.IsNullOrEmpty(), "OptionalName for ViewModels is not implemented");
            _registeredPrefabs[model.GetType()] = Object.Instantiate(model);
            _registeredPrefabs[model.GetType()].gameObject.SetActive(false);
            return (TViewModel) _registeredPrefabs[model.GetType()];
        }

        public TViewModel Build<TViewModel>(string optName) where TViewModel : ViewModelMonoBehaviour
        {
            Assert.IsTrue(optName.IsNullOrEmpty(), "OptionalName for ViewModels is not implemented");
            if (_registeredPrefabs.TryGetValue(typeof(TViewModel), out var viewModel))
            {
                viewModel.gameObject.SetActive(true);
                return (TViewModel) viewModel;
            }

            return default;
        }

        public void Free(ViewModelMonoBehaviour viewModel)
        {
            viewModel.gameObject.SetActive(false);
        }
    }
}