﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ScriptableObjectArchitecture.Editor
{
    public class PropertyIterator : IPropertyIterator
    {
        public PropertyIterator(SerializedProperty property)
        {
            iterator = property.Copy();
            endProperty = iterator.GetEndProperty();

            if (iterator.propertyType == SerializedPropertyType.Generic)
                iterator.NextVisible(true);
        }

        protected readonly SerializedProperty iterator;
        protected readonly SerializedProperty endProperty;

        /*
         * Improvement:
         *
         * This field causes the warning "The field is assigned but its value is never used" so disabling that warning.
         * In the future, it's possible to remove this field, but need to verify that it's actually not used
         */
#pragma warning disable CS0414
        private bool consumeChildren;
#pragma warning restore CS0414
        private int parentDepth;

        public virtual bool Next()
        {
            bool nextVisible = false;
            if(IsSingleLine(iterator))
            {
                parentDepth = iterator.depth;
                nextVisible = iterator.NextVisible(false);
            }
            else
            {
                nextVisible = iterator.NextVisible(true);
            }

            if (!CanDraw())
                return false;

            if(nextVisible)
            {
                if (iterator.propertyType == SerializedPropertyType.Generic)
                    nextVisible = iterator.NextVisible(true);
            }

            return nextVisible && CanDraw();
        }
        public virtual void End()
        {
        }
        private void UpdateState(SerializedProperty property)
        {
            if (IsSingleLine(iterator))
            {
                parentDepth = iterator.depth;
                consumeChildren = true;
            }
        }
        private bool CanDraw()
        {
            return !SerializedProperty.EqualContents(iterator, endProperty);
        }
        private bool IsSingleLine(SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Vector3:
                case SerializedPropertyType.Vector2:
                case SerializedPropertyType.Vector3Int:
                case SerializedPropertyType.Vector2Int:
                case SerializedPropertyType.Vector4:
                case SerializedPropertyType.Quaternion:
                case SerializedPropertyType.Rect:
                case SerializedPropertyType.RectInt:
                case SerializedPropertyType.Bounds:
                case SerializedPropertyType.BoundsInt:
                    return true;
            }

            return false;
        }
        private bool NextVisible()
        {
            return iterator.NextVisible(true);
        }
    }
}
