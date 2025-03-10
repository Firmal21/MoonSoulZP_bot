﻿//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text.RegularExpressions;
//using System.Threading;
//using System.Threading.Tasks;
//using Telegram.Bot;
//using Telegram.Bot.Polling;
//using Telegram.Bot.Types;
//using Telegram.Bot.Types.Enums;

//class Program
//{
//    static async Task Main()
//    {
//        var botClient = new TelegramBotClient("7890997396:AAEB6ZJZf-J2H5Q_CVyhRp-adLrKEzkNnww");

//        var cancellationTokenSource = new CancellationTokenSource();

//        // Настройка приема обновлений
//        var receiverOptions = new ReceiverOptions
//        {
//            AllowedUpdates = Array.Empty<UpdateType>() // Получать все типы обновлений
//        };

//        botClient.StartReceiving(
//            HandleUpdateAsync,
//            HandleErrorAsync,
//            receiverOptions,
//            cancellationToken: cancellationTokenSource.Token);

//        Console.WriteLine("Бот запущен. Нажмите Enter, чтобы выйти.");
//        Console.ReadLine();

//        // Остановка приема обновлений
//        cancellationTokenSource.Cancel();
//    }

//    static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
//    {
//        if (update.Type != UpdateType.Message || update.Message.Type != MessageType.Text)
//            return;

//        double summ = 0;
//        //var messageexit = update.message;
//        //var message = (regex.replace(update.message.text, @"\s+", " ").trim()).split(new char[] { '\n' }, stringsplitoptions.removeemptyentries);  


//        var message = update.Message;
//        var secMessage =  message.Text.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
//        List<string> messageList = new List<string>();

//        foreach( string str in secMessage)
//        {

//            messageList.Add(Regex.Replace(str, @"\s+", " ").Trim());
//        }


//        foreach (string str in messageList)
//        {
//            string[] words = str.Split(' ');
//            int lastElement = words.Length - 1;
//            int secondLastElement = words.Length - 2;
//            //int mnojitel = 1;

//            var index = words[secondLastElement].Substring(1);

//            bool result = int.TryParse(index, out var correctIndex);

//            //mnojitel = correctIndex;
//            if(result == true)
//            summ += Int32.Parse(words[lastElement]) * correctIndex;

//            else
//                summ += Int32.Parse(words[lastElement]) * 1;
//        }


//        if(summ != 0)
//        await botClient.SendTextMessageAsync(message.Chat, Convert.ToString(summ), cancellationToken: cancellationToken);

//        else
//        await botClient.SendTextMessageAsync(message.Chat, "Ошибочка, проверь входные данные", cancellationToken: cancellationToken);

//        return;
//    }


//    static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
//    {
//        Console.WriteLine($"Ошибка: {exception.Message}");
//        return Task.CompletedTask;
//    }
//}






//тут будет пункт 5.4
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace MoonSoulZPBot
{
    public class TestBot
    {

        private static ITelegramBotClient _botClient;
        private static ReceiverOptions _receiverOptions;
        private static string _typeOfCount;

        static async Task Main()
        {
            _botClient = new TelegramBotClient("7890997396:AAEB6ZJZf-J2H5Q_CVyhRp-adLrKEzkNnww"); // Укажите свой токен
            _receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new[] { UpdateType.Message }
            };

            var cts = new CancellationTokenSource();
            _botClient.StartReceiving(UpdateHandler, ErrorHandler, _receiverOptions, cts.Token);

            var me = await _botClient.GetMeAsync();
            Console.WriteLine($"Бот запущен: {me.Username}");
            await Task.Delay(-1);
        }

        private static Task ErrorHandler(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            Console.WriteLine($"Ошибка: {exception.Message}");
            return Task.CompletedTask;
        }

        [Obsolete]
        private static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {


            try
            {
                if (update.Type == UpdateType.Message)
                {
                    var message = update.Message;
                    var chat = message.Chat;

                    if (message.Type == MessageType.Text)
                    {
                        if (message.Text == "/start" || message.Text == "Посчитать зарплату" || message.Text == "Э" || message.Text == "э")
                        {

                            var removeKeyboard = new ReplyKeyboardRemove();
                            var startButton = new ReplyKeyboardMarkup(
                                new List<KeyboardButton[]>
                                {
                                    new KeyboardButton[] { new KeyboardButton("Цена указана за одно украшение") },
                                    new KeyboardButton[] {new KeyboardButton("Цена уже умножена") }
                                })
                            {
                                ResizeKeyboard = true

                            };

                            await botClient.SendTextMessageAsync(
                                chat.Id,
                                "Привет Мариночка-Вонючечка, сейчас посчитаем сколько миллионов ты вынесла за сегодня\n ",
                                replyMarkup: startButton);


                            await botClient.SendTextMessageAsync(
                                chat.Id,
                                "Выбери в меню каким способом ты записывала цену!",
                                replyMarkup: startButton);
                            return;
                        }

                        //Первое действие

                        else if (message.Text == "Цена указана за одно украшение")
                        {
                            var removeKeyboard = new ReplyKeyboardRemove();
                            await botClient.SendTextMessageAsync(chat.Id, "Читож, ты выбрала, что цена указана за одно украшение\n Кидай мне свой список.", replyMarkup: removeKeyboard);
                            _typeOfCount = "price for one";
                            return;

                        }

                        else if (message.Text == "Цена уже умножена")
                        {
                            var removeKeyboard = new ReplyKeyboardRemove();
                            await botClient.SendTextMessageAsync(chat.Id, "Читож, ты выбрала, что цена уже умножена\n Кидай мне свой список.", replyMarkup: removeKeyboard);
                            _typeOfCount = "price for all";
                            return;
                        }

                        if (_typeOfCount == "price for one")
                        {
                            PriceForOne(botClient, update, cancellationToken);
                            _typeOfCount = null;
                            return;
                        }

                        if (_typeOfCount == "price for all")
                        {
                            PriceForAll(botClient, update, cancellationToken);
                            _typeOfCount = null;
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex}");
            }
        }


        private static async Task PriceForOne(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            double summ = 0;
            var messageList1 = update.Message;
            var secMessage = messageList1.Text.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> messageList = new List<string>();


            foreach (string str in secMessage)
            {

                messageList.Add(Regex.Replace(str, @"\s+", " ").Trim());
            }


            foreach (string str in messageList)
            {
                string[] words = str.Split(' ');
                int lastElement = words.Length - 1;
                int secondLastElement = words.Length - 2;
                //int mnojitel = 1;

                var index = words[secondLastElement].Substring(1);

                bool resultCount = int.TryParse(index, out var correctIndex);

                bool resultPrice = int.TryParse(words[lastElement], out var correctPrice);
                if (resultPrice == false)
                {
                    await botClient.SendTextMessageAsync(messageList1.Chat, "Ошибочка, проверь входные данные", cancellationToken: cancellationToken);
                    return;
                }


                //mnojitel = correctIndex;
                if (resultCount == true)
                    summ += Int32.Parse(words[lastElement]) * correctIndex;

                else
                    summ += Int32.Parse(words[lastElement]) * 1;
            }


            if (summ > 0)
            {
                await botClient.SendTextMessageAsync(messageList1.Chat, text: "Сейчас посчитаем, сколько ты зарабоатла\n " + summ, cancellationToken: cancellationToken);

                //await botClient.SendTextMessageAsync(messageList1.Chat, Convert.ToString(summ), cancellationToken: cancellationToken);

                if (summ <= 1000)
                    await botClient.SendTextMessageAsync(messageList1.Chat, "МДЭЭЭ, СПАСИБО ВЕРОНИКА ЗА ТРЕЩЕТКУ", cancellationToken: cancellationToken);

                if (summ <= 3000 && summ > 1000)
                    await botClient.SendTextMessageAsync(messageList1.Chat, "ТЬФУ ТЫ, ВСЕ ДОРОГИЕ УКРАШЕНИЯ УКРАЛА КАТЯ", cancellationToken: cancellationToken);

                if (summ <= 4000 && summ > 3000)
                    await botClient.SendTextMessageAsync(messageList1.Chat, "Сегодня заработано нормально деньжат", cancellationToken: cancellationToken);

                if (summ <= 5000 && summ > 4000)
                    await botClient.SendTextMessageAsync(messageList1.Chat, "Ну даешь!", cancellationToken: cancellationToken);

                if (summ >= 5000)
                    await botClient.SendTextMessageAsync(messageList1.Chat, "За это надо выпить!", cancellationToken: cancellationToken);

                var firstStartButton = new ReplyKeyboardMarkup(
                               new List<KeyboardButton[]>
                               {
                                    new KeyboardButton[] { new KeyboardButton("Посчитать зарплату") },
                               })
                {
                    ResizeKeyboard = true

                };


                await botClient.SendTextMessageAsync(messageList1.Chat
                                ,
                                "Считаем зепку по новой?",
                                replyMarkup: firstStartButton);
                return;


            }

            else
                await botClient.SendTextMessageAsync(messageList1.Chat, "Ошибка, проверь входные данные", cancellationToken: cancellationToken);
            return;
        }
        

        private static async Task PriceForAll(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            double summ = 0;
            var messageList1 = update.Message;
            var secMessage = messageList1.Text.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> messageList = new List<string>();

            foreach (string str in secMessage)
            {
                messageList.Add(Regex.Replace(str, @"\s+", " ").Trim());
            }


            foreach (string str in messageList)
            {
                string[] words = str.Split(' ');
                int lastElement = words.Length - 1;
                int secondLastElement = words.Length - 2;

                var index = words[secondLastElement].Substring(1);



                bool resultCount = int.TryParse(words[lastElement], out var correctPrice);
                if (resultCount == true)
                    summ += correctPrice;
                else
                {
                    await botClient.SendTextMessageAsync(messageList1.Chat, "Ошибочка, проверь входные данные", cancellationToken: cancellationToken);
                    return;
                }



            }
            if (summ > 0)
            {
                await botClient.SendTextMessageAsync(messageList1.Chat, text: "Сейчас посчитаем, сколько ты зарабоатла\n " + summ, cancellationToken: cancellationToken);

                //await botClient.SendTextMessageAsync(messageList1.Chat, Convert.ToString(summ), cancellationToken: cancellationToken);

                if (summ <= 1000)
                    await botClient.SendTextMessageAsync(messageList1.Chat, "МДЭЭЭ, СПАСИБО ВЕРОНИКА ЗА ТРЕЩЕТКУ", cancellationToken: cancellationToken);

                if (summ <= 3000 && summ > 1000)
                    await botClient.SendTextMessageAsync(messageList1.Chat, "ТЬФУ ТЫ, ВСЕ ДОРОГИЕ УКРАШЕНИЯ УКРАЛА КАТЯ", cancellationToken: cancellationToken);

                if (summ <= 4000 && summ > 3000)
                    await botClient.SendTextMessageAsync(messageList1.Chat, "Сегодня заработано нормально деньжат", cancellationToken: cancellationToken);

                if (summ <= 5000 && summ > 4000)
                    await botClient.SendTextMessageAsync(messageList1.Chat, "Ну даешь!", cancellationToken: cancellationToken);

                if (summ >= 5000)
                    await botClient.SendTextMessageAsync(messageList1.Chat, "За это надо выпить!", cancellationToken: cancellationToken);

                var firstStartButton = new ReplyKeyboardMarkup(
                               new List<KeyboardButton[]>
                               {
                                    new KeyboardButton[] { new KeyboardButton("Посчитать зарплату") },
                               })
                {
                    ResizeKeyboard = true

                };


                await botClient.SendTextMessageAsync(messageList1.Chat
                                ,
                                "Считаем зепку по новой?",
                                replyMarkup: firstStartButton);
                return;


            }

            else
                await botClient.SendTextMessageAsync(messageList1.Chat, "Ошибка, проверь входные данные", cancellationToken: cancellationToken);
            return;
        }


    }
}


/*
private static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    // Обязательно ставим блок try-catch, чтобы наш бот не "падал" в случае каких-либо ошибок
    try
    {
        // Сразу же ставим конструкцию switch, чтобы обрабатывать приходящие Update
        switch (update.Type)
        {
            case UpdateType.Message:
                {
                    // Эта переменная будет содержать в себе все связанное с сообщениями
                    var message = update.Message;

                    // From - это от кого пришло сообщение (или любой другой Update)
                    var user = message.From;

                    // Выводим на экран то, что пишут нашему боту, а также небольшую информацию об отправителе
                    Console.WriteLine($"{user.FirstName} ({user.Id}) написал сообщение: {message.Text}");

                    // Chat - содержит всю информацию о чате
                    var chat = message.Chat;

                    // Добавляем проверку на тип Message
                    switch (message.Type)
                    {
                        // Тут понятно, текстовый тип
                        case MessageType.Text:
                            {
                                // тут обрабатываем команду /start, остальные аналогично
                                if (message.Text == "/start")
                                {
                                    await botClient.SendTextMessageAsync(
                                        chat.Id,
                                        "Выбери клавиатуру:\n" +
                                        "/inline\n" +
                                        "/reply\n");
                                    return;
                                }

                                if (message.Text == "/inline")
                                {
                                    // Тут создаем нашу клавиатуру
                                    var inlineKeyboard = new InlineKeyboardMarkup(
                                        new List<InlineKeyboardButton[]>() // здесь создаем лист (массив), который содрежит в себе массив из класса кнопок
                                        {
                                        // Каждый новый массив - это дополнительные строки,
                                        // а каждая дополнительная кнопка в массиве - это добавление ряда

                                        new InlineKeyboardButton[] // тут создаем массив кнопок
                                        {
                                            InlineKeyboardButton.WithUrl("Это кнопка с сайтом", "https://habr.com/"),
                                            InlineKeyboardButton.WithCallbackData("А это просто кнопка", "button1"),
                                        },
                                        new InlineKeyboardButton[]
                                        {
                                            InlineKeyboardButton.WithCallbackData("Тут еще одна", "button2"),
                                            InlineKeyboardButton.WithCallbackData("И здесь", "button3"),
                                        },
                                        });

                                    await botClient.SendTextMessageAsync(
                                        chat.Id,
                                        "Это inline клавиатура!",
                                        replyMarkup: inlineKeyboard); // Все клавиатуры передаются в параметр replyMarkup

                                    return;
                                }

                                if (message.Text == "/reply")
                                {
                                    // Тут все аналогично Inline клавиатуре, только меняются классы
                                    // НО! Тут потребуется дополнительно указать один параметр, чтобы
                                    // клавиатура выглядела нормально, а не как абы что

                                    var replyKeyboard = new ReplyKeyboardMarkup(
                                        new List<KeyboardButton[]>()
                                        {
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("Привет!"),
                                            new KeyboardButton("Пока!"),
                                        },
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("Позвони мне!")
                                        },
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("Напиши моему соседу!")
                                        }
                                        })
                                    {
                                        // автоматическое изменение размера клавиатуры, если не стоит true,
                                        // тогда клавиатура растягивается чуть ли не до луны,
                                        // проверить можете сами
                                        ResizeKeyboard = true,
                                    };

                                    await botClient.SendTextMessageAsync(
                                        chat.Id,
                                        "Это reply клавиатура!",
                                        replyMarkup: replyKeyboard); // опять передаем клавиатуру в параметр replyMarkup

                                    return;
                                }

                                if (message.Text == "Позвони мне!")
                                {
                                    await botClient.SendTextMessageAsync(
                                        chat.Id,
                                        "Хорошо, присылай номер!",
                                        replyToMessageId: message.MessageId);

                                    return;
                                }

                                if (message.Text == "Напиши моему соседу!")
                                {
                                    await botClient.SendTextMessageAsync(
                                        chat.Id,
                                        "А самому что, трудно что-ли ?",
                                        replyToMessageId: message.MessageId);

                                    return;
                                }

                                return;
                            }

                        // Добавил default , чтобы показать вам разницу типов Message
                        default:
                            {
                                await botClient.SendTextMessageAsync(
                                    chat.Id,
                                    "Используй только текст!");
                                return;
                            }
                    }

                    return;
                }

            case UpdateType.CallbackQuery:
                {
                    // Переменная, которая будет содержать в себе всю информацию о кнопке, которую нажали
                    var callbackQuery = update.CallbackQuery;

                    // Аналогично и с Message мы можем получить информацию о чате, о пользователе и т.д.
                    var user = callbackQuery.From;

                    // Выводим на экран нажатие кнопки
                    Console.WriteLine($"{user.FirstName} ({user.Id}) нажал на кнопку: {callbackQuery.Data}");

                    // Вот тут нужно уже быть немножко внимательным и не путаться!
                    // Мы пишем не callbackQuery.Chat , а callbackQuery.Message.Chat , так как
                    // кнопка привязана к сообщению, то мы берем информацию от сообщения.
                    var chat = callbackQuery.Message.Chat;

                    // Добавляем блок switch для проверки кнопок
                    switch (callbackQuery.Data)
                    {
                        // Data - это придуманный нами id кнопки, мы его указывали в параметре
                        // callbackData при создании кнопок. У меня это button1, button2 и button3

                        case "button1":
                            {
                                // В этом типе клавиатуры обязательно нужно использовать следующий метод
                                await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                                // Для того, чтобы отправить телеграмму запрос, что мы нажали на кнопку

                                await botClient.SendTextMessageAsync(
                                    chat.Id,
                                    $"Вы нажали на {callbackQuery.Data}");
                                return;
                            }

                        case "button2":
                            {
                                // А здесь мы добавляем наш сообственный текст, который заменит слово "загрузка", когда мы нажмем на кнопку
                                await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, "Тут может быть ваш текст!");

                                await botClient.SendTextMessageAsync(
                                    chat.Id,
                                    $"Вы нажали на {callbackQuery.Data}");
                                return;
                            }

                        case "button3":
                            {
                                // А тут мы добавили еще showAlert, чтобы отобразить пользователю полноценное окно
                                await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, "А это полноэкранный текст!", showAlert: true);

                                await botClient.SendTextMessageAsync(
                                    chat.Id,
                                    $"Вы нажали на {callbackQuery.Data}");
                                return;
                            }
                    }

                    return;
                }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.ToString());
    }
}
*/