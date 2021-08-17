using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Music_user_bot
{
    class Whitelist
    {
        public static List<ulong> white_list { get; private set; }
        public static ulong ownerID { get; set; }
        public Whitelist(params ulong[] userIDs) {
            if (IsNullOrEmpty(userIDs))
            {
                white_list = new List<ulong> {};
            }
            white_list = userIDs.ToList();
        }
        public static void AddToWL(ulong userID)
        {
            white_list.Add(userID);
        }
        public static void RemoveFromWL(ulong userID)
        {
            if (white_list.FindIndex(a => a == userID) != -1)
            {
                white_list.Remove(userID);
            }
        }
        public static bool IsNullOrEmpty(ulong[] array)
        {
            if (array == null || array.Length == 0)
                return true;
            else
                return array.All(item => item == 0);
        }
    }
}
