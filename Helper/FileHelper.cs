using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace 素材合成.Helper
{
    public class FileHelper
    {

        static public void AppandLog(string Info, string Level = "Error")
        {
            
         
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"######{Level}-{System.DateTime.Now.ToString("F")}######");
            sb.AppendLine(Info);
            sb.AppendLine();
            SaveFile_Append("log.txt", sb.ToString());
            FileInfo fileInfo = new FileInfo("log.txt");
            if (fileInfo.Length > 100 * 1024 * 1024)
            {
                fileInfo.Delete();
            }
        }

        /// <summary>
        /// 读取文件 以Show OpenFileDialog 方式
        /// </summary>
        /// <returns></returns>
        static public byte[] OpenFile()
        {
            OpenFileDialog openData = new OpenFileDialog();
            openData.Filter = "所有文件|*";
            openData.InitialDirectory = System.AppDomain.CurrentDomain.BaseDirectory.Split(new string[] { "\\bin" }, StringSplitOptions.RemoveEmptyEntries)[0];
            openData.ShowDialog();
            if (openData.FileName != "")
            {
                using (FileStream fsRead = new FileStream(openData.FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    long len = fsRead.Seek(0, SeekOrigin.End);
                    fsRead.Seek(0, SeekOrigin.Begin);
                    byte[] buffer = new byte[len];
                    int count = fsRead.Read(buffer, 0, buffer.Length);
                    return buffer;
                }
            }
            return BitConverter.GetBytes(0);
        }
        static public byte[] OpenFile_Dialog(string initial_directory)
        {

            OpenFileDialog openData = new OpenFileDialog();
            openData.Filter = "所有文件|*";
            openData.InitialDirectory = initial_directory;
            openData.ShowDialog();
            if (openData.FileName != "")
            {
                using (FileStream fsRead = new FileStream(openData.FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    long len = fsRead.Seek(0, SeekOrigin.End);
                    fsRead.Seek(0, SeekOrigin.Begin);
                    byte[] buffer = new byte[len];
                    int count = fsRead.Read(buffer, 0, buffer.Length);
                    return buffer;
                }
            }
            else
            {
                return BitConverter.GetBytes(0);
            }
        }
        static public string OpenFile_getPath(string initial_directory)
        {

            OpenFileDialog openData = new OpenFileDialog();
            openData.Filter = "所有文件|*.*";
            openData.InitialDirectory = initial_directory;
            openData.ShowDialog();
            if (openData.FileName != "")
            {
                return openData.FileName;
            }
            else
            {
                return "";
            }
        }
        static public Stream OpenFile_getStream(string initial_directory)
        {

            OpenFileDialog openData = new OpenFileDialog();
            openData.Filter = "所有文件|*";
            openData.InitialDirectory = initial_directory;
            openData.ShowDialog();
            if (openData.FileName != "")
            {
                using (Stream fsRead = new FileStream(openData.FileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    return fsRead;
                }
            }
            else
            {
                return null;
            }
        }
        static public byte[] OpenFile(string file_path)
        {

            using (FileStream fsRead = new FileStream(file_path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                try
                {
                    long len = fsRead.Seek(0, SeekOrigin.End);
                    fsRead.Seek(0, SeekOrigin.Begin);
                    byte[] buffer = new byte[len];
                    int count = fsRead.Read(buffer, 0, buffer.Length);
                    return buffer;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
                finally
                {
                    fsRead.Close();
                }
            }
            return null;
        }
        static public byte[] OpenFile(string file_path, int buffer_size, int offset)
        {
            byte[] buffer = new byte[buffer_size];
            using (FileStream fsRead = new FileStream(file_path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                try
                {
                    fsRead.Seek(offset, SeekOrigin.Begin);
                    int count = fsRead.Read(buffer, 0, buffer.Length);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
                finally
                {
                    fsRead.Close();
                }
            }
            return buffer;
        }
        static public long getLength(string file_path)
        {
            using (FileStream fsRead = new FileStream(file_path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                try
                {
                    long len = fsRead.Seek(0, SeekOrigin.End);
                    fsRead.Seek(0, SeekOrigin.Begin);
                    return len;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
                finally
                {
                    fsRead.Close();
                }
            }
            return 0;
        }
        /// <summary>
        /// Show SaveFileDialog 方式
        /// </summary>
        /// <param name="Event"></param>
        static public void SaveFile(CancelEventHandler Event)
        {
            SaveFileDialog openfile = new SaveFileDialog();
            openfile.Title = "选择保存的文件路径";
            string[] buffer_path = System.AppDomain.CurrentDomain.BaseDirectory.Split(new string[] { "\\bin" }, StringSplitOptions.RemoveEmptyEntries);
            openfile.InitialDirectory = buffer_path[0];
            openfile.Filter = "Excel文件|*.xlsx |TXT文件|*.txt";
            //添加保存按钮触发事件
            openfile.FileOk += Event;

            openfile.ShowDialog();
        }
        static public void SaveFile(string file_name, FileMode mode, FileAccess access, string data, int size)
        {
            using (FileStream fsWrite = new FileStream(file_name, mode, access, FileShare.ReadWrite))
            {
                byte[] buffer = new byte[size + 10];
                buffer = Encoding.UTF8.GetBytes(data + "\r\n");
                fsWrite.Write(buffer, 0, buffer.Length);
            }
        }
        static public void SaveFile_Append(string file_name, string data)
        {
            using (FileStream fsWrite = new FileStream(file_name, FileMode.Append, FileAccess.Write, FileShare.Write))
            {
                byte[] buffer =  Encoding.UTF8.GetBytes(data);
                fsWrite.Write(buffer, 0, buffer.Length);
            }
        }

        static public void SaveFile_Append(string file_name, byte[] data, int dataLength)
        {
            using (FileStream fsWrite = new FileStream(file_name, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            {
                try
                {
                    fsWrite.Write(data, 0, dataLength);
                }
                catch (Exception e)
                {
                    MessageBox.Show("写入文件出错:" + e.Message);
                }
                finally
                {
                    fsWrite.Flush();
                    fsWrite.Close();
                }
            }
        }
        static public void SaveFile_Create(string file_name, string data)
        {
            using (FileStream fsWrite = new FileStream(file_name, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                byte[]  buffer = Encoding.UTF8.GetBytes(data);
                fsWrite.Write(buffer, 0, buffer.Length);
            }
        }

        static public List<string> readFileToList(string fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            List<string> list = new List<string>();
            StreamReader m_streamReader = new StreamReader(fs, System.Text.Encoding.Default);//中文乱码加上System.Text.Encoding.Default,或则 System.Text.Encoding.GetEncoding("GB2312")
            //使用StreamReader类来读取文件
            m_streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
            // 从数据流中读取每一行，直到文件的最后一行，并在richTextBox1中显示出内容


            string strLine = m_streamReader.ReadLine();
            while (strLine != null)
            {
                list.Add(strLine);
                strLine = m_streamReader.ReadLine();
            }
            //关闭此StreamReader对象
            m_streamReader.Close();
            return list;


        }
        static public void Delete(string path)
        {
            File.Delete( path);
        }
    }
}
