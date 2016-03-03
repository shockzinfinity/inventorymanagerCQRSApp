using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManager.Infrastructure.Azure
{
    public static class RowKeyConstants
    {
        public const string RowKeyUpperLimit = "9999999999";

        public const string UnpublishedRowKeyPrefix = "Unpublished_";
        public const string UnpublishedRowKeyPrefixUpperLimit = "Unpublished`";
    }
}
