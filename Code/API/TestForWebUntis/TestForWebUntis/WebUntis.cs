/*
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

using System.Net;
using System.IO;
using System.Threading;


namespace DoDownload
{
    public class RequestState
    {
        // This class stores the State of the request.
        const int BUFFER_SIZE = 1024;
        public StringBuilder requestData;
        public byte[] BufferRead;
        public HttpWebRequest request;
        public HttpWebResponse response;
        public Stream streamResponse;
        public RequestState()
        {
            BufferRead = new byte[BUFFER_SIZE];
            requestData = new StringBuilder("");
            request = null;
            streamResponse = null;
        }
    }

    class HttpWebRequest_BeginGetResponse
    {
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        const int BUFFER_SIZE = 1024;
        const int DefaultTimeout = 2 * 60 * 1000;

        // Abort the request if the timer fires.
        private static void TimeoutCallback(object state, bool timedOut)
        {
            if (timedOut)
            {
                HttpWebRequest request = state as HttpWebRequest;
                if (request != null)
                {
                    request.Abort();
                }
            }
        }

        //public static void Main()
        public void start()
        {

            try
            {
                // Create a HttpWebrequest object to the desired URL. 
                HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create("http://www.contoso.com");


                /**
                * If you are behind a firewall and you do not have your browser proxy setup
                * you need to use the following proxy creation code.

                    // Create a proxy object.
                    WebProxy myProxy = new WebProxy();

                    // Associate a new Uri object to the _wProxy object, using the proxy address
                    // selected by the user.
                    myProxy.Address = new Uri("http://myproxy");


                    // Finally, initialize the Web request object proxy property with the _wProxy
                    // object.
                    myHttpWebRequest.Proxy=myProxy;
                ***/

                // Create an instance of the RequestState and assign the previous myHttpWebRequest
                // object to its request field.  
                RequestState myRequestState = new RequestState();
                myRequestState.request = myHttpWebRequest;


                // Start the asynchronous request.
                IAsyncResult result =
                (IAsyncResult)myHttpWebRequest.BeginGetResponse(new AsyncCallback(RespCallback), myRequestState);

                // this line implements the timeout, if there is a timeout, the callback fires and the request becomes aborted
                ThreadPool.RegisterWaitForSingleObject(result.AsyncWaitHandle, new WaitOrTimerCallback(TimeoutCallback), myHttpWebRequest, DefaultTimeout, true);

                // The response came in the allowed time. The work processing will happen in the 
                // callback function.
                allDone.WaitOne();

                // Release the HttpWebResponse resource.
                myRequestState.response.Close();
            }
            catch (WebException e)
            {
                System.Diagnostics.Debug.WriteLine("\nMain Exception raised!");
                System.Diagnostics.Debug.WriteLine("\nMessage:{0}", e.Message);
                System.Diagnostics.Debug.WriteLine("\nStatus:{0}", e.Status);
                System.Diagnostics.Debug.WriteLine("Press any key to continue..........");
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("\nMain Exception raised!");
                // System.Diagnostics.Debug.WriteLine("Source :{0} " , e.Source);
                System.Diagnostics.Debug.WriteLine("Message :{0} ", e.Message);
                System.Diagnostics.Debug.WriteLine("Press any key to continue..........");
                //System.Diagnostics.Debug.Read();
            }
        }
        private static void RespCallback(IAsyncResult asynchronousResult)
        {
            try
            {
                // State of request is asynchronous.
                RequestState myRequestState = (RequestState)asynchronousResult.AsyncState;
                HttpWebRequest myHttpWebRequest = myRequestState.request;
                myRequestState.response = (HttpWebResponse)myHttpWebRequest.EndGetResponse(asynchronousResult);

                // Read the response into a Stream object.
                Stream responseStream = myRequestState.response.GetResponseStream();
                myRequestState.streamResponse = responseStream;

                // Begin the Reading of the contents of the HTML page and print it to the System.Diagnostics.Debug.
                IAsyncResult asynchronousInputRead = responseStream.BeginRead(myRequestState.BufferRead, 0, BUFFER_SIZE, new AsyncCallback(ReadCallBack), myRequestState);
                return;
            }
            catch (WebException e)
            {
                System.Diagnostics.Debug.WriteLine("\nRespCallback Exception raised!");
                System.Diagnostics.Debug.WriteLine("\nMessage:{0}", e.Message);
                System.Diagnostics.Debug.WriteLine("\nStatus:{0}", e.Status);
            }
            allDone.Set();
        }
        private static void ReadCallBack(IAsyncResult asyncResult)
        {
            try
            {

                RequestState myRequestState = (RequestState)asyncResult.AsyncState;
                Stream responseStream = myRequestState.streamResponse;
                int read = responseStream.EndRead(asyncResult);
                // Read the HTML page and then print it to the System.Diagnostics.Debug.
                if (read > 0)
                {
                    // use UTF8 instead of ASCII
                    myRequestState.requestData.Append(Encoding.UTF8.GetString(myRequestState.BufferRead, 0, read));
                    //myRequestState.requestData.Append(Encoding.ASCII.GetString(myRequestState.BufferRead, 0, read));
                    IAsyncResult asynchronousResult = responseStream.BeginRead(myRequestState.BufferRead, 0, BUFFER_SIZE, new AsyncCallback(ReadCallBack), myRequestState);
                    return;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("\nThe contents of the Html page are : ");
                    if (myRequestState.requestData.Length > 1)
                    {
                        string stringContent;
                        stringContent = myRequestState.requestData.ToString();
                        System.Diagnostics.Debug.WriteLine(stringContent);
                    }
                    System.Diagnostics.Debug.WriteLine("Press any key to continue..........");
                   // System.Diagnostics.Debug.ReadLine();

                    responseStream.Close();
                }

            }
            catch (WebException e)
            {
                System.Diagnostics.Debug.WriteLine("\nReadCallBack Exception raised!");
                System.Diagnostics.Debug.WriteLine("\nMessage:{0}", e.Message);
                System.Diagnostics.Debug.WriteLine("\nStatus:{0}", e.Status);
            }
            allDone.Set();

        }
        /*static void Main(string[] args)
        {
        }*/
    }
}
