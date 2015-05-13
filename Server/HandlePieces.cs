using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server {
    class HandlePieces {

        public static void GeneratePieces() {
            int c=0;
            int n=1;
            string zero="0";
            for(int i=0 ; i<=105 ; i++) {
                if(i==52||i==105) {
                    c=0;
                    Server.pieces.Insert(i, 500);
                    continue;
                }
                if(i<52) {
                    if(i%13!=0) {
                        n++;
                    }
                    if(i%13==0) {
                        c++;
                        n=1;
                    }
                } else {
                    if(( i-1 )%13!=0) {
                        n++;
                    }
                    if(( i-1 )%13==0) {
                        c++;
                        n=1;
                    }
                }
                if(n<10) {
                    zero="0";
                } else {
                    zero="";
                }
                Server.pieces.Insert(i, Int32.Parse(c.ToString()+zero+n.ToString()));
            }
        }

    }
}
