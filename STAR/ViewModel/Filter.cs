using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace STAR.ViewModel
{
    class Filter
    {
        private string filterType;
        private List<Packet> filteredPackets;
        private bool[] portFilter;
        private bool[] errorFilter;

        public Filter()
        {
            Clear();
        }

        public void filter(List<Packet> packets)
        {
            int pointer;

            if (packets.Count == 0)
            {
                return;
            }

            filterType += "Ports: ";
            for (pointer = 0; pointer < 8; pointer++)
            {
                if (portFilter[pointer] == true)
                {
                    filterType += pointer;
                    filterType += ", ";
                }
            }
            filterType = filterType.Remove(filterType.Length - 2, 2);
            filterType += "Errors: ";
            for (pointer = 0; pointer < 3; pointer++)
            {
                if (errorFilter[pointer] == true)
                {
                    filterType += pointer;
                    filterType += ", ";
                }
            }
            filterType = filterType.Remove(filterType.Length - 2, 2);

            foreach (Packet packet in packets)
            {
                if (packet is DataPacket)
                {
                    for (pointer = 0; pointer < 8; pointer++)
                    {
                        if (portFilter[pointer] == true)
                        {
                            if (Convert.ToInt32(packet.Port) == pointer)
                            {
                                filteredPackets.Add(packet);
                            }
                        }
                    }

                }
            }
        }
        public void Clear()
        {
            filterType = "";
            filteredPackets.Clear();
        }
        public void print()
        {
            Console.WriteLine();
            for (int pointer = 0; pointer < filteredPackets.Count; pointer++)
            {
                Console.WriteLine(filteredPackets);
            }
            Console.WriteLine();
        }

    }
}
