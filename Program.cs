using ImageMagick;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.Json;

internal class Program
{
    static object locker = new object();
    static int width = 1000, height = 1000;
    static Bitmap bmp = new Bitmap(width, height);
    static Random rand = new Random();

    private static void CreateBmp()
    {
        lock (locker)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {

                    int a = rand.Next(256);
                    int r = rand.Next(256);
                    int g = rand.Next(256);
                    int b = rand.Next(256);
                    bmp.SetPixel(x, y, Color.FromArgb(a, r, g, b));
                }
            }
            bmp.Save("RandomImage.png");
        }

    }
    private static void CreateJson()
    {
        lock (locker)
        {

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color ColorA = new Color();
                    Color ColorB = new Color();
                    if (x < width - 1)
                    {
                        ColorA = bmp.GetPixel(x, y);
                        ColorB = bmp.GetPixel(x + 1, y);
                    }
                    else if (y < height-1)
                    {
                        ColorA = bmp.GetPixel(x, y);
                        ColorB = bmp.GetPixel(0, y+1);
                    }
                        int r1 = ColorA.R;
                        int g1 = ColorA.G;
                        int b1 = ColorA.B;
                        int r2 = ColorB.R;
                        int g2 = ColorB.G;
                        int b2 = ColorB.B;
                        double distance = Math.Sqrt(Math.Pow(r1 - r2, 2) + Math.Pow(g1 - g2, 2) + Math.Pow(b1 - b2, 2)); //2.3 - примерно соответствует минимально различимому для человеческого глаза отличию между цветами (wiki)
                    if (distance >= 2 && distance <= 2.5)
                        {
                            Console.WriteLine(x.ToString() + ", " + y.ToString());
                            Console.WriteLine(r1.ToString() + ", " + g1.ToString() + ", " + b1.ToString());
                            string str = JsonSerializer.Serialize<Color>(ColorA);
                            str += "\n" + x.ToString() + " - " + y.ToString() + "\n";
                            File.AppendAllText("jsonData.json", str);

                        }
                    
                }

            }
        }
        File.AppendAllText("jsonData.json", "===========================\n");
    }

    static void Main(string[] args)
    {
        Thread t1 = new Thread(CreateBmp);
        t1.Start();
        Thread t2 = new Thread(CreateJson);
        t2.Start();
    }

}

