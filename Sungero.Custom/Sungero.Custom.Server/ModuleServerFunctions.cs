using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using RabbitMQ.Client.Events;

namespace Sungero.Custom.Server
{
  public class ModuleFunctions
  {

    /// <summary>
    /// Функция проверки соответствия пользователя на принадлежность группе "Допуск к маркетинговым документам"
    /// </summary> 
    [Public]
    public bool DostumMarketing(Sungero.CoreEntities.IUser user)
    {
      var Rol = Roles.GetAll(r => r.Name == "Допуск к маркетинговым документам").FirstOrDefault();
      bool marker = false;
      if (Rol != null)
      {
        var Empl = Sungero.Company.Employees.GetAll(t => t.Login == user.Login).FirstOrDefault();
        if (Empl != null)
        {
          if (Rol.RecipientLinks.Count > 0)
          {
            foreach (var elem in Rol.RecipientLinks)
            {
              if (elem.Member.Id == Empl.Id)
                marker = true;
            }
          }
        }
      }
      return marker;
    } 
    
    
    [Public(WebApiRequestType = RequestType.Get)]
		public string GetTestValFromRabbitMQ()
		{
			string originalMessage = string.Empty;
			
			// Создать очередь сообщений для справочника «Организации»
			string exchangeName = "adm.companies.changeElements.drx";
			string queueName = "adm.companies.changeElements.drx";
			string routingKey = "adm.companies.drx";
			
			ConnectionFactory factory = new ConnectionFactory();
      // 'virtualhost=rxhost;hostname=192.168.115.10;username=rxhost;password=rxhost;exchange=directumrx_sberdev;Port=5672'
			factory.UserName =  "rxhost";
			factory.Password =  "rxhost";
			factory.VirtualHost = "rxhost";
			factory.HostName = "localhost"; // hostName;
			factory.Port = 5672;

			// Создание соединения
			IConnection conn = factory.CreateConnection();
			
			// Создание канала
			IModel channel = GetRabbitChannel(conn, exchangeName, queueName, routingKey); // channel = conn.CreateModel();

			// Получение сообщения
			BasicGetResult result = channel.BasicGet(queueName, false);
			if (result == null)
			{
				// В настоящее время нет доступных сообщений.
				originalMessage = "0";
			}
			else
			{
			  //byte[] body = result.Body;
			  originalMessage = result.MessageCount.ToString(); //  System.Text.Encoding.UTF8.GetString(body);
			}
			
			// Отключение соединения
			channel.Close();
			conn.Close();
			
			return originalMessage;
		}
		
		private IModel GetRabbitChannel(IConnection conn, string exchangeName, string queueName, string routingKey)
		{
			IModel model = conn.CreateModel();
			model.ExchangeDeclare(exchangeName, ExchangeType.Direct);
			model.QueueDeclare(queueName, false, false, false, null);
			model.QueueBind(queueName, exchangeName, routingKey, null);
			return model;
		}
		
  }
}