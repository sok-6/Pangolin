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
        private DataValue _defaultValue;

        public IReadOnlyList<Token> TokenList => _innerList;
        public int CurrentTokenIndex { get; private set; }

        public TokenQueue()
        {
            CurrentTokenIndex = 0;
            _innerList = new List<Token>();
            _defaultValue = DataValue.Falsey;
        }

        public TokenQueue(DataValue defaultValue, params Token[] tokens)
        {
            CurrentTokenIndex = 0;
            _innerList = new List<Token>(tokens);
            _defaultValue = defaultValue;
        }

        public void Enqueue(Token item)
        {
            _innerList.Add(item);
        }

        public virtual DataValue DequeueAndEvaluate()
        {
            // Check if run out of tokens
            if (CurrentTokenIndex == _innerList.Count)
            {
                return _defaultValue;
            }

            // 'Dequeue' and evaluate
            var currentToken = _innerList[CurrentTokenIndex];
            CurrentTokenIndex++;
            return currentToken.Evaluate(this);
        }
    }
}
