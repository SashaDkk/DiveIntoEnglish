using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.NoUnity
{
    /// <summary>
    /// Передвигатель элементов
    /// </summary>
    internal class NodeMover
    {
        /// <summary>
        /// Является UI-объектом
        /// </summary>
        private RectTransform _uiObject;
        
        /// <summary>
        /// Действие, прикрепленное к объекту (какое угодно)
        /// </summary>
        public Action Tag;
        
        /// <summary>
        /// Объект, который надо двигать
        /// </summary>
        public readonly GameObject ObjectToMove;

        /// <summary>
        /// Направление, в котором надо двигать объект
        /// </summary>
        public readonly Vector2 VectorToMove;

        /// <summary>
        /// Движение по оси x завершено
        /// </summary>
        private bool _moveXDone;

        /// <summary>
        /// Движение по оси Y завершено
        /// </summary>
        private bool _moveYDone;

        /// <summary>
        /// Скорость движения
        /// </summary>
        private float _speed;

        /// <summary>
        /// Выполнить движение
        /// </summary>
        /// <returns>true, если еще есть куда двигать</returns>
        public bool Update()
        {
            if (_moveXDone && _moveYDone)
                return false;
            if (_uiObject == null)
            {
                var prevPosition = ObjectToMove.transform.position;
                var xDelta = 0f;
                if (!_moveXDone)
                {
                    xDelta = _speed * (prevPosition.x > VectorToMove.x ? -Time.fixedDeltaTime : Time.fixedDeltaTime);
                    var realDelta = prevPosition.x - VectorToMove.x;
                    if (Mathf.Abs(realDelta) < Mathf.Abs(xDelta))
                    {
                        _moveXDone = true;
                        xDelta = realDelta;
                    }
                }
                var yDelta = 0f;
                if (!_moveYDone)
                {
                    yDelta = _speed * (prevPosition.y > VectorToMove.y ? -Time.fixedDeltaTime : Time.fixedDeltaTime);
                    var realDelta = prevPosition.y - VectorToMove.y;
                    if (Mathf.Abs(realDelta) < Mathf.Abs(yDelta))
                    {
                        _moveYDone = true;
                        yDelta = realDelta;
                    }
                }
                ObjectToMove.transform.position = new Vector3(prevPosition.x + xDelta, prevPosition.y + yDelta, prevPosition.z);
            }
            else
            {
                var prevPosition = _uiObject.anchoredPosition;
                var xDelta = 0f;
                if (!_moveXDone)
                {
                    xDelta = _speed * (prevPosition.x > VectorToMove.x ? -Time.fixedDeltaTime : Time.fixedDeltaTime);
                    var realDelta = prevPosition.x - VectorToMove.x;
                    if (Mathf.Abs(realDelta) < Mathf.Abs(xDelta))
                    {
                        _moveXDone = true;
                        xDelta = realDelta;
                    }
                }
                var yDelta = 0f;
                if (!_moveYDone)
                {
                    yDelta = _speed * (prevPosition.y > VectorToMove.y ? -Time.fixedDeltaTime : Time.fixedDeltaTime);
                    var realDelta = prevPosition.y - VectorToMove.y;
                    if (Mathf.Abs(realDelta) < Mathf.Abs(yDelta))
                    {
                        _moveYDone = true;
                        yDelta = realDelta;
                    }
                }
                _uiObject.anchoredPosition = new Vector2(prevPosition.x + xDelta, prevPosition.y + yDelta);
            }
            return true;
        }

        /// <summary>
        /// Создание
        /// </summary>
        /// <param name="objectToMove"></param>
        /// <param name="vectorToMove"></param>
        public NodeMover(GameObject objectToMove, float? newX, float? newY, float speed = 1f)
        {
            ObjectToMove = objectToMove;
            var position = ObjectToMove.transform.position;
            VectorToMove = new Vector2(newX ?? position.x, newY ?? position.y);
            _speed = speed;
            _uiObject = objectToMove.GetComponent<RectTransform>();
        }
    }
}
