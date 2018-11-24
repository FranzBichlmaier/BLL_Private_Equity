using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_DataModels
{
    public class Message
    {
        private int id;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        private string messageText;

        public string MessageText
        {
            get { return messageText; }
            set { messageText = value; }
        }
        private DateTime creationDateTime;

        public DateTime CreationDateTime
        {
            get { return creationDateTime; }
            set { creationDateTime = value; }
        }
        private string creationType;

        public string CreationType
        {
            get { return creationType; }
            set { creationType = value; }
        }



    }
}
