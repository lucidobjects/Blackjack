using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackjack
{
    public interface ICardContainer
    {
        List<Card> Cards { get; }
    }
}
