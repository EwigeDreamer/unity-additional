using System;
using System.Collections.Generic;
using System.Linq;
using ED.Additional.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ED.Additional.Components.UI
{
    public sealed class LoopScrollRect : ScrollRect
    {
        [SerializeField] private ScrollDirection _scrollDirection;

        private int _scrollAxisIndex;
        private HorizontalOrVerticalLayoutGroup _contentLayoutGroup;
        private LoopList<RectTransform> _items = new();
        private readonly List<float> _offsetSumStack = new();
        private float _offsetSum;
        private Canvas _mainCanvas;
        private Vector2 _dragOffset;
        private bool _dragging;
        private Vector2 _prevDragPosition;
        private Vector2 _currDragPosition;
        private Vector2 _dragVelocity;
        private float _loopNormalizedPosition;

        public IReadOnlyList<RectTransform> LoopContentItems => _items;
        public IReadOnlyList<RectTransform> LoopContentRawItems => _items.RawList;

        public float LoopNormalizedPosition
        {
            get => _loopNormalizedPosition;
            set
            {
                velocity = Vector2.zero;
                var size = GetContentSize();
                var position = -size * (value % 1f);
                SetContentAnchoredPosition(position);
            }
        }

        private void RefreshLoopNormalizedPosition()
        {
            var viewBounds = new Bounds(viewport.rect.center, viewport.rect.size);
            var contentBounds = m_ContentBounds;
            _loopNormalizedPosition = (viewBounds.min[_scrollAxisIndex] - contentBounds.min[_scrollAxisIndex] + _offsetSum) / contentBounds.size.x;
        }

        protected override void Awake()
        {
            base.Awake();

            if (Application.isPlaying)
            {
                if (content != null)
                {
                    content.anchorMin = content.anchorMax = content.pivot = Vector2.zero;

                    if (!content.TryGetComponent(out _contentLayoutGroup))
                    {
                        _contentLayoutGroup = content.gameObject.AddComponent(GetLayoutRequiredLayoutGroupType()) as HorizontalOrVerticalLayoutGroup;
                        _contentLayoutGroup!.reverseArrangement = _scrollDirection switch
                        {
                            ScrollDirection.Horizontal => false,
                            ScrollDirection.Vertical => true,
                            _ => default
                        };
                    }
                }

                _mainCanvas = GetComponentInParent<Canvas>();
                _scrollAxisIndex = (int)_scrollDirection;
            }
        }

        protected override void Start()
        {
            base.Start();

            if (Application.isPlaying)
            {
                RefreshItems();
            }
        }

        public void RefreshItems()
        {
            var children = Enumerable.Range(0, content.childCount)
                                     .Select(i => content.GetChild(i))
                                     .Where(c => c.gameObject.activeSelf)
                                     .OfType<RectTransform>();
            _items = new LoopList<RectTransform>(children);
            _offsetSumStack.Clear();
            SetContentAnchoredPosition(Vector2.zero);
        }

        protected override void OnTransformParentChanged()
        {
            _mainCanvas = GetComponentInParent<Canvas>();
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);
            _dragging = true;

            if (inertia)
            {
                TryGetLocalDragPosition(eventData, out _prevDragPosition);
                _dragVelocity = Vector2.zero;
            }
        }

        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);
            
            if (!_dragging)
                return;

            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            if (!IsActive())
                return;

            if (inertia)
                TryGetLocalDragPosition(eventData, out _currDragPosition);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);
            
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            
            _dragOffset = Vector2.zero;
            _dragging = false;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _dragOffset = Vector2.zero;
            _dragging = false;
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();
            
            if (Application.isPlaying)
            {
                if (Time.unscaledDeltaTime > 0f)
                {
                    if (_dragging && inertia)
                    {
                        CalculateDragVelocity();
                    }
                }
            }
        }

        private void CalculateDragVelocity()
        {
            var deltaTime = Time.unscaledDeltaTime;
            var newVelocity = (_currDragPosition - _prevDragPosition) / deltaTime;
            _dragVelocity = Vector2.Lerp(_dragVelocity, newVelocity, deltaTime * 10f);
            _prevDragPosition = _currDragPosition;
            velocity = _dragVelocity;
        }

        private bool TryGetLocalDragPosition(PointerEventData eventData, out Vector2 localDragPosition)
        {
            return RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    viewRect,
                    eventData.position,
                    eventData.pressEventCamera,
                    out localDragPosition);
        }

        private Type GetLayoutRequiredLayoutGroupType()
        {
            return _scrollDirection switch
            {
                ScrollDirection.Horizontal => typeof(HorizontalLayoutGroup),
                ScrollDirection.Vertical => typeof(VerticalLayoutGroup),
                _ => throw new InvalidOperationException(),
            };
        }

        private Vector2 GetScrollDirectionAxis()
        {
            return _scrollDirection switch
            {
                ScrollDirection.Horizontal => Vector2.right,
                ScrollDirection.Vertical => Vector2.up,
                _ => throw new InvalidOperationException(),
            };
        }

        protected override void SetNormalizedPosition(float value, int axis) { }


        protected override void SetContentAnchoredPosition(Vector2 position)
        {
            var viewSize = GetViewportSize();
            var contentSize = GetContentSize();
            
            if (ValidateContentSize(viewSize, contentSize))
            {
                if (_dragging || velocity != Vector2.zero)
                {
                    if (IsContentOverInViewport(viewSize, contentSize, position + _dragOffset, out var steps))
                    {
                        RearrangeContentGroup(steps, out var offset);

                        if (_dragging)
                            _dragOffset += offset;
                        else
                            position += offset;
                    }

                    base.SetContentAnchoredPosition(position + _dragOffset);
                }
                else
                {
                    position += GetScrollDirectionAxis() * _offsetSum;
                    if (IsContentOverInViewport(viewSize, contentSize, position, out var steps))
                    {
                        RearrangeContentGroup(steps, out var offset);
                        position += offset;
                    }
                    base.SetContentAnchoredPosition(position);
                }
            }

            RefreshLoopNormalizedPosition();
        }

        public Vector2 GetViewportSize()
        {
            return RectTransformUtility.PixelAdjustRect(viewport, _mainCanvas).size;
        }

        public Vector2 GetContentSize()
        {
            if (content == null)
                throw new InvalidOperationException($"{nameof(content)} is null!");
            if (_contentLayoutGroup == null)
                throw new InvalidOperationException($"{nameof(_contentLayoutGroup)} is null!");
            return content.sizeDelta + Vector2.one * _contentLayoutGroup.spacing;
        }

        private bool ValidateContentSize(Vector2 viewSize, Vector2 contentSize)
        {
            var itemMax = Vector2.zero;
            foreach (var item in _items)
                itemMax = Vector2.Max(itemMax, item.sizeDelta);
            contentSize -= itemMax;
            return contentSize[_scrollAxisIndex] > viewSize[_scrollAxisIndex];
        }

        private bool IsContentOverInViewport(Vector2 viewSize, Vector2 contentSize, Vector2 position, out int stepsToReach)
        {
            stepsToReach = 0;
            var min = position;
            var max = position + contentSize;

            var minAxis = min[_scrollAxisIndex];
            var maxAxis = max[_scrollAxisIndex];
            var viewSizeAxis = viewSize[_scrollAxisIndex];

            //check left side
            var index = _items.Count - 1;
            while (minAxis > 0f)
            {
                minAxis -= GetItemOffsetScalar(_items[index--]);
                stepsToReach--;
            }

            if (stepsToReach != 0)
                return true;
            
            //check right side
            index = 0;
            while (maxAxis < viewSizeAxis)
            {
                maxAxis += GetItemOffsetScalar(_items[index--]);
                stepsToReach++;
            }
            
            return stepsToReach != 0;
        }

        private void RearrangeContentGroup(int steps, out Vector2 offset)
        {
            offset = Vector2.zero;
            var count = Math.Abs(steps);
            for (int i = 0; i < count; ++i)
            {
                if (steps < 0)
                {
                    var item = _items[^1];
                    offset -= GetItemOffset(item);

                    _items.StartIndex--;

                    if (_offsetSumStack.Count > 0 && _offsetSum > 0f) PopOffset();
                    else PushOffset(-GetItemOffsetScalar(item));
                }
                else
                {
                    var item = _items[0];
                    offset += GetItemOffset(item);

                    _items.StartIndex++;

                    if (_offsetSumStack.Count > 0 && _offsetSum < 0f) PopOffset();
                    else PushOffset(GetItemOffsetScalar(item));
                }
            }
            
            UpdateContentGroupOrder();
        }

        private void PushOffset(float value)
        {
            _offsetSumStack.Add(value);
            RefreshOffsetSum();
        }
        private void PopOffset()
        {
            if (_offsetSumStack.Count == 0) return;
            _offsetSumStack.RemoveAt(_offsetSumStack.Count - 1);
            RefreshOffsetSum();
        }
        private void RefreshOffsetSum()
        {
            _offsetSum = 0;
            foreach (var item in _offsetSumStack)
                _offsetSum += item;
        }

        private void UpdateContentGroupOrder()
        {
            foreach (var item in _items)
                item.SetAsLastSibling();
        }

        private Vector2 GetItemOffset(RectTransform item)
        {
            return _scrollDirection switch
            {
                ScrollDirection.Horizontal => new Vector2(item.sizeDelta.x + _contentLayoutGroup.spacing, 0f),
                ScrollDirection.Vertical => new Vector2(0f, item.sizeDelta.y + _contentLayoutGroup.spacing),
                _ => throw new InvalidOperationException(),
            };
        }

        private float GetItemOffsetScalar(RectTransform item)
        {
            return item.sizeDelta[_scrollAxisIndex] + _contentLayoutGroup.spacing;
        }
        
        private enum ScrollDirection : byte
        {
            Horizontal = 0,
            Vertical   = 1,
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            horizontal = _scrollDirection == ScrollDirection.Horizontal;
            vertical = _scrollDirection == ScrollDirection.Vertical;
            movementType = MovementType.Unrestricted;

            if (content != null)
            {
                content.anchorMin = content.anchorMax = content.pivot = Vector2.zero;
                
                UnityEditor.EditorApplication.delayCall += () => //We can't add components inside OnValidate
                {
                    if (content == null)
                        return;
                    
                    if (content.TryGetComponent<HorizontalOrVerticalLayoutGroup>(out var group))
                    {
                        if (!ValidateLayoutGroup(group))
                        {
                            var json = JsonUtility.ToJson(group);
                            DestroyImmediate(group);
                            group = content.gameObject.AddComponent(GetLayoutRequiredLayoutGroupType()) as HorizontalOrVerticalLayoutGroup;
                            UnityEditorInternal.ComponentUtility.MoveComponentUp(group);
                            JsonUtility.FromJsonOverwrite(json, group);
                            group!.reverseArrangement = _scrollDirection switch
                            {
                                ScrollDirection.Horizontal => false,
                                ScrollDirection.Vertical => true,
                                _ => default
                            };
                        }
                    }
                    else
                    {
                        content.gameObject.AddComponent(GetLayoutRequiredLayoutGroupType());
                    }

                    if (!content.TryGetComponent<ContentSizeFitter>(out var fitter))
                    {
                        fitter = content.gameObject.AddComponent<ContentSizeFitter>();
                        fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                    }
                };
            }
            
            base.OnValidate();
        }

        private bool ValidateLayoutGroup(HorizontalOrVerticalLayoutGroup group)
        {
            return _scrollDirection switch
            {
                ScrollDirection.Horizontal => group is HorizontalLayoutGroup,
                ScrollDirection.Vertical => group is VerticalLayoutGroup,
                _ => throw new InvalidOperationException(),
            };
        }
        
        [UnityEditor.CustomEditor(typeof(LoopScrollRect))]
        private class LoopScrollRectEditor : UnityEditor.UI.ScrollRectEditor
        {
            private UnityEditor.SerializedProperty _directionProperty;
            private GUIStyle _titleStyle;
            private float _titleHeight;
            
            protected override void OnEnable()
            {
                base.OnEnable();
                _directionProperty = serializedObject.FindProperty(nameof(_scrollDirection));
            }

            public override void OnInspectorGUI()
            {
                TitleField($"{nameof(LoopScrollRect)} properties:");
                UnityEditor.EditorGUILayout.PropertyField(_directionProperty);
                
                UnityEditor.EditorGUILayout.GetControlRect();
                
                TitleField($"Base {nameof(ScrollRect)} properties:");
                serializedObject.ApplyModifiedProperties();
                base.OnInspectorGUI();
            }

            private void InitTitleSkin()
            {
                if (_titleStyle == null)
                {
                    _titleStyle = new GUIStyle(GUI.skin.label);
                    _titleStyle.fontStyle = FontStyle.BoldAndItalic;
                    _titleStyle.fontSize *= 2;
                    _titleHeight = UnityEditor.EditorGUIUtility.singleLineHeight * 2f;
                }
            }

            private void TitleField(string text)
            {
                InitTitleSkin();
                var rect = UnityEditor.EditorGUILayout.GetControlRect(false, _titleHeight);
                UnityEditor.EditorGUI.LabelField(rect, text, _titleStyle);
            }
        }
#endif
    }
}