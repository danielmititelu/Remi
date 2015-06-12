using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server {
    class UniqueRandom {
        private readonly List<int> _currentList;
        private readonly Random _random=new Random();
        public UniqueRandom(IEnumerable<int> seed) {
            _currentList=new List<int>(seed);
        }

        public int Next() {
            if(_currentList.Count==0) {
                throw new ApplicationException("No more numbers");
            }
            int i=_random.Next (_currentList.Count);
            //int i=0;
            int result=_currentList[i];
            _currentList.RemoveAt(i);
            return result;
        }
        public IEnumerable<int> Next14() {
            for(int i=0 ; i<=13 ;i++ ) {
                yield return Next();
            }
        }
    }
}
