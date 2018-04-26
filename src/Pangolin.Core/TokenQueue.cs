using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Core
{
    public class TokenQueue
    {
        private List<Token> _innerList;
        private int _currentIndex;
        private DataValue _defaultValue;

        public int Count => _innerList.Count - _currentIndex;

        public TokenQueue(DataValue defaultValue, params Token[] tokens)
        {
            _currentIndex = 0;
            _innerList = new List<Token>(tokens);
            _defaultValue = defaultValue;
        }

        public void Enqueue(Token item)
        {
            _innerList.Add(item);
        }

        public DataValue DequeueAndEvaluate()
        {
            // Check if run out of tokens
            if (_currentIndex == _innerList.Count)
            {
                return _defaultValue;
            }

            // 'Dequeue' and evaluate
            var currentToken = _innerList[_currentIndex];
            _currentIndex++;
            return currentToken.Evaluate(this);
        }
    }
}
