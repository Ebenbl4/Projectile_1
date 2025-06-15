using System.Drawing;

namespace Medic
{
    public static class SharedData
    {
        // Поля для хранения загруженных пользователем рентгенограмм
        public static Bitmap Radiograph_1 { get; set; }
        public static Bitmap Radiograph_2 { get; set; }
        public static Bitmap Radiograph_3 { get; set; }
        public static Bitmap Radiograph_4 { get; set; }

        // Поля для хранения всех даннных, учавствующих при вынесении вердикта

        public static double Dh;
        public static double Da;
        public static double R;
        public static double ISh;
        public static double Dist_A;
        public static double Dist_B;
        public static double Dist_D;
        public static double Dist_W;
        public static double Okano;
        public static double Angle_A;
        public static double Angle_B;
        public static double Angle_V;
        public static double Angle_Viberg;
        public static double Angle_D;
        public static double Angle_E;
        public static double ISA;
        public static double ICAS;
        public static double AK;
        public static double UOB;
        public static double SLN;
        public static double SPN_1;
        public static double SDLN;
        public static double HP;
        public static double BP;
        public static double trans;
        public static string PHIO;
        public static int year;
        public static int age;

        // Методы для освобождения ресурсов, когда изображение больше не нужно
        // (например, при закрытии приложения)
        public static void DisposeSharedPic1()
        {
            Radiograph_1?.Dispose();
            Radiograph_1 = null;
        }

        public static void DisposeSharedPic2()
        {
            Radiograph_2?.Dispose();
            Radiograph_2 = null;
        }
        public static void DisposeSharedPic3()
        {
            Radiograph_3?.Dispose();
            Radiograph_3 = null;
        }

        public static void DisposeSharedPic4()
        {
            Radiograph_4?.Dispose();
            Radiograph_4 = null;
        }
    }
}
