using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBotWPF
{
  public class Server : INotifyPropertyChanged
  {
    //
    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      try
      {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    private int? myProperty;
    public int? MyProperty
    {
      get { return myProperty; }
      private set
      {
        myProperty = value;
        OnPropertyChanged();
      }
    }

    public async Task InitializeAsync()
    {
      await Task.Delay(100);
      MyProperty = 13;
    }


    private static HttpListener listener;
    static Boolean running;
    public Boolean Running
    {
      get { return running; }
      set
      {
        if (running != value)
        {
          running = value;
          OnPropertyChanged();
        }
      }
    }
    static string status;
    public string Status
    {
      get { return status; }
      set
      {
        if (status != value)
        {
          status = value;
          OnPropertyChanged();
        }
      }
    }


    public void Start(string[] prefixes)
    {
      //PropertyChanged = new PropertyChangedEventHandler({ });
      listener = new HttpListener();
      foreach (string s in prefixes)
      {
        listener.Prefixes.Add(s);
      }
      try
      {
        listener.Start();
        listener.BeginGetContext(OnContext, listener);
        Status = "Waiting for requests";
        Running = true;
      }
      catch (Exception ex)
      {
        Status = "Exception:" + ex.ToString();
        Running = false;
        //throw;
      }
      //Console.ReadLine();
    }
    private void OnContext(IAsyncResult ar)
    {
      var context = listener.EndGetContext(ar);
      listener.BeginGetContext(OnContext, listener);
      HttpListenerRequest request = context.Request;
      HttpListenerResponse response = context.Response;// Obtain a response object.
      // Construct a response.
      string responseString = "<HTML><BODY>";
      foreach (var cookieKey in request.QueryString.AllKeys)
      {
        responseString += "Key: " + cookieKey + ", Value:" + request.QueryString[cookieKey] + "<br/>";
        if (cookieKey == "code")
          //lock (new object())
          //{
          Status = request.QueryString[cookieKey];
        //}
        //Interlocked.
      }
      //NotifyPropertyChanged("Status");
      responseString += "<a href='#' onclick='close();return false;'>close</a></BODY></HTML>";
      byte[] buffer = Encoding.UTF8.GetBytes(responseString);
      // Get a response stream and write the response to it.
      response.ContentLength64 = buffer.Length;
      Stream output = response.OutputStream;
      output.Write(buffer, 0, buffer.Length);
      output.Close();
    }

  }
}
