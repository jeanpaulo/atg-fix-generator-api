using static System.Net.Mime.MediaTypeNames;
using System.Drawing;
using System.IO;
using QuickFix;
using OrderGenerator.API.DTO;
using QuickFix.Fields;

namespace OrderGenerator.API;

public class FIXCommunicator : MessageCracker, IApplication
{
    Session _session = null;
    private int ClOrdID = 0;

    #region overrides
    public void OnCreate(SessionID sessionID)
    {
        _session = Session.LookupSession(sessionID);
    }
    public void OnLogon(SessionID sessionID) { Console.WriteLine("Logon - " + sessionID.ToString()); }
    public void OnLogout(SessionID sessionID) { Console.WriteLine("Logout - " + sessionID.ToString()); }

    public void FromAdmin(Message message, SessionID sessionID) { }
    public void ToAdmin(Message message, SessionID sessionID) { }

    public void FromApp(Message message, SessionID sessionID)
    {
        Console.WriteLine("IN:  " + message.ToString());
        try
        {
            Crack(message, sessionID);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public void ToApp(Message message, SessionID sessionID)
    {
        try
        {
            bool possDupFlag = false;
            if (message.Header.IsSetField(QuickFix.Fields.Tags.PossDupFlag))
            {
                possDupFlag = QuickFix.Fields.Converters.BoolConverter.Convert(
                    message.Header.GetString(QuickFix.Fields.Tags.PossDupFlag));
            }
            if (possDupFlag)
                throw new DoNotSend();
        }
        catch (FieldNotFoundException ex)
        {
            Console.WriteLine($"a------ {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"b------ {ex.Message}");
        }

        Console.WriteLine("OUT: " + message.ToString());
    }
    #endregion

    #region MessageCracker handlers
    public string OnMessage(QuickFix.FIX44.ExecutionReport m, SessionID s)
    {
        return "Received execution report";
    }

    public void OnMessage(QuickFix.FIX44.OrderCancelReject m, SessionID s)
    {
        Console.WriteLine("Received order cancel reject");
    }
    #endregion


    public async Task Run(OrderGeneratorRequestDTO request)
    {
        try
        {
            var newOrderSingle = new QuickFix.FIX44.NewOrderSingle
            (
                new ClOrdID(GenClOrdID()),
                new Symbol(request.Simbolo),
                new Side(request.Lado),
                new TransactTime(DateTime.Now),
                new OrdType(OrdType.MARKET)
            );

            newOrderSingle.Set(new TimeInForce(TimeInForce.DAY));
            newOrderSingle.Set(new OrderQty(request.Quantidade));
            newOrderSingle.Price = new Price(request.Preco);

            if (newOrderSingle != null)
            {
                newOrderSingle.Header.GetString(Tags.BeginString);

                SendMessage(newOrderSingle);
            }
        }
        catch (Exception ex)
        {
            var error = ex.Message;
            throw;
        }


    }

    private void SendMessage(Message m)
    {
        if (_session != null)
        {
            _session.Send(m);
        }
        else
        {
            Console.WriteLine("Can't send message: session not created.");
        }
    }

    private string GenClOrdID() { return (++ClOrdID).ToString(); }

}
