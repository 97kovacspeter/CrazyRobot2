using Robot.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robot2.Model
{
    public class RobotEventArgs : EventArgs
    {
        private Int32 _position;

        public Int32 GetPosition { get { return _position; } }

        public RobotEventArgs( Int32 position )
        {
            _position = position;

        }
    }
}
