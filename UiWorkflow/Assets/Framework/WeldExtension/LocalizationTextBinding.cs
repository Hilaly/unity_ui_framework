using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityWeld.Binding;
using UnityWeld.Binding.Internal;

namespace Framework.WeldExtension
{
    [RequireComponent(typeof(Text))]
    public class LocalizationTextBinding : AbstractMemberBinding
    {
        [SerializeField] private string _textFormat = "{0}";
        [SerializeField] private List<string> _viewModelPropertyName = new List<string>();
        private PropertyWatcher[] _viewModelWatchers;
        private Action _sync;
        private object[] _localized;

        public List<string> ViewModelProperties => _viewModelPropertyName;

        public string LocalizationKey
        {
            get => _textFormat;
            set => _textFormat = value;
        }

        public override void Connect()
        {
            try
            {
                ParseViewEndPointReference(typeof(Text).FullName + ".text", out var memberName, out var view);

                var sources = _viewModelPropertyName.Select(u => MakeViewModelEndPoint(u, null, null)).ToList();
                var viewProperty = new PropertyEndPoint(view, memberName, null, null, "view", this);

                _viewModelWatchers = sources.Select(source => source.Watch(UpdateView)).ToArray();
                _localized = sources.ConvertAll(s => s.GetValue() ?? string.Empty).ToArray();
                _sync = () =>
                {
                    try
                    {
                        if (sources.Count == 0)
                        {
                            viewProperty.SetValue(_textFormat);
                        }
                        else
                        {
                            for (int i = 0; i < sources.Count; i++)
                            {
                                try
                                {
                                    var value = sources[i]?.GetValue() ?? _localized[i];
                                    _localized[i] = value;
                                }
                                catch (Exception e)
                                {
                                    Debug.LogWarning(
                                        $"LocalizationException Key={_textFormat} Args='{string.Join(",", _localized.Select(u => u.ToString()))}', error when try to get value for {_viewModelPropertyName[i]} on {gameObject.name}");
                                    throw;
                                }
                            }

                            var resultText = string.Format(_textFormat, _localized);
                            if (view != null)
                                viewProperty.SetValue(resultText);
                        }
                    }
                    catch (TargetInvocationException tie)
                    {
                        Debug.LogWarning(
                            $"LocalizationException Key={_textFormat} Args='{string.Join(",", _localized.Select(u => u.ToString()))}' on {gameObject.name}");
                        Debug.LogException(tie.GetBaseException());
                        Disconnect();
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning(
                            $"LocalizationException Key={_textFormat} Args='{string.Join(",", _localized.Select(u => u.ToString()))}' on {gameObject.name}");
                        Debug.LogException(e);
                        Disconnect();
                    }
                };


                UpdateView();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Debug.LogError($"Localization binding error on '{this.name}': {e}");
                Disconnect();
            }
        }

        private void UpdateView()
        {
            _sync?.Invoke();
        }

        public override void Disconnect()
        {
            _sync = null;

            if (_viewModelWatchers == null)
                return;

            foreach (var watcher in _viewModelWatchers)
                watcher.Dispose();
            _viewModelWatchers = null;
        }
    }
}