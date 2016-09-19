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
                    PacketView errorPacketView = new PacketView(packet);
                    string errorType = errorPacketView.Message;
                    if (errorFilter[0] == true) //I.E. If there is a parity error
                    {
                        if (errorType == "Parity")
                        {
                            filteredPackets.Add(packet);
                        }
                    }else if (errorFilter[1] == true)
                    {
                        if (errorType == "Disconnect")
                        {
                            filteredPackets.Add(packet);
                        }
                    }
                }
            }
        }

        public void Clear()
        {
            filteredPackets.Clear();
            for (int pointer = 0; pointer < 8; pointer++)
            {
                errorFilter[pointer] = false;
            }
            for (int counter = 0; counter < 3; counter++)
            {
                portFilter[counter] = false;
            }
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
