namespace Mirle.ASRS.WCS.Model.DataAccess
{
    public struct IOtype
    {
        public const int NormalstorIn = 1;
        public const int NormalstoreOut = 2;
        public const int R2R = 5;
        //public const int EmptyStoreIn = 6; //只有A4用到
        //public const int EmptyStroeOut = 7;//只有A4用到
        //public const int EmptyStoreOutbyWMS = 11;//由WMS發起，單純出空棧板

        public const int Cycle = 31;
        public const int ErpStoreIn = 12;
        public const int NoTktStoreIn = 13;
        public const int ProduceStoreIn = 11;
        public const int abnormalOUT = 9;
        public const int ERPtktabnormalOUT = 61;
    }
}