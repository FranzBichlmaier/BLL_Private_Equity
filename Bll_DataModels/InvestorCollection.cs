using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_DataModels
{
    public class InvestorCollection: ObservableCollection<Investor>
    {
        protected override void InsertItem(int index, Investor item)
        {
            this.AdoptItem( item);
            base.InsertItem(index, item);
        }
        protected override void RemoveItem(int index)
        {
            this.DiscardItem(this[index]);
            base.RemoveItem(index);
        }
        protected override void SetItem(int index, Investor item)
        {
            this.AdoptItem(item);
            base.SetItem(index, item);
        }
        protected override void ClearItems()
        {
            foreach (Investor item in this)
            {
                this.DiscardItem(item);
            }
            base.ClearItems();
        }

        private void AdoptItem(Investor item)
        {
            item.SetOwner(this);
        }
        private void DiscardItem(Investor item)
        {
            item.SetOwner(null);
        }
    }
}
