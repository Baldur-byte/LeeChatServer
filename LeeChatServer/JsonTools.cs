namespace LeeChatServer
{
    public static class JsonTools
    {
        private static bool isOpening = false;

        public static string Read(string filepath)
        {
            string json = "";
            while (isOpening)
            {
            }
            isOpening = true;
            try
            {
                if (!File.Exists(filepath))
                {
                    Console.WriteLine("文件不存在！");
                    File.Create(filepath);
                }
                using (StreamReader sr = new StreamReader(filepath))
                {
                    json = sr.ReadToEnd();
                    sr.Close();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("读文件出错：" + ex.Message);
            }
            finally
            {
                isOpening = false;
            }

            return json;
        }

        public static void Write(string filepath, string content)
        {
            while (isOpening)
            {
                //Console.WriteLine("写文件出错，文件已经打开");
                //return;
            }
            isOpening = true;
            try
            {
                if (File.Exists(filepath)) File.Delete(filepath);
                File.Create(filepath).Dispose();
                using (StreamWriter sr = new StreamWriter(filepath))
                {
                    sr.Write(content);
                    sr.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("写文件出错：" + ex.Message);
            }
            finally 
            { 
                isOpening = false; 
            }
        }
    }
}
