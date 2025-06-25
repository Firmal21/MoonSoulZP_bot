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

namespace ConsoleApp1
{
    public class TestBot
    {

        private static ITelegramBotClient _botClient;
        private static ReceiverOptions _receiverOptions;
        private static string _typeOfCount;
        private static double _totalSumm;
        private static ReplyKeyboardRemove _removeKeyboard;
        private static double _lastSumm;
        private static List<string> _finalList = new List<string>();
        private static string _messageText;

        static async Task Main()
        {
            _botClient = new TelegramBotClient("7563563149:AAEDX-6wVHX7KRH1lq2Vth373nK5xuNc-dY"); 
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
                    var chatId = message.Chat.Id;

                    if (message.Type == MessageType.Text)
                    {
                        if (message.Text == "/start" || message.Text == "Посчитать зарплату снова" || message.Text == "Э" || message.Text == "э")
                        {
                            _totalSumm = 0;
                            var removeKeyboard = new ReplyKeyboardRemove();
                            var startButton = new ReplyKeyboardMarkup(
                                new List<KeyboardButton[]>
                                {
                                    new KeyboardButton[] { new KeyboardButton("Вести список во время робика") },
                                    new KeyboardButton[] {new KeyboardButton("У меня уже есть список") }
                                })
                            {
                                ResizeKeyboard = true

                            };

                            await botClient.SendTextMessageAsync(
                                chat.Id,
                                "Привет, Мариночка-Вонючечка, сейчас посчитаем, сколько миллионов ты вынесла за сегодня\n" +
                                "выбери в меню как ты хочешь посчитать зарплату.",
                                replyMarkup: startButton);
                            return;
                        }

                        if (message.Text == "Вести список во время робика")
                        {
                            var startListButton = new ReplyKeyboardMarkup(
                            new List<KeyboardButton[]>
                            {
                                    new KeyboardButton[] { new KeyboardButton("Удалить прошлое украшение") },
                                    new KeyboardButton[] {new KeyboardButton("Закончить") }
                            })
                            {
                                ResizeKeyboard = true

                            };

                            await botClient.SendTextMessageAsync(
                                chat.Id,
                                "Хорошо, кидай мне украшения по ходу дела, но обязательно пиши их название.\n" +
                                "Когда захочешь закончить нажми в меню на кнопку.\n" +
                                "Также там есть кнопка удалить предыдущее украшение, если что-то пошло не так",
                                replyMarkup: startListButton);
                            _typeOfCount = "price during job";
                            return;
                        }





                        if (message.Text == "У меня уже есть список")
                        {
                            var startListButton = new ReplyKeyboardMarkup(
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
                                "Итак, у тебя есть список, но как же в нем записана цена за украшение?\n Выбери в меню",
                                replyMarkup: startListButton);


                            //await botClient.SendTextMessageAsync(
                            //    chat.Id,
                            //    "Выбери в меню каким способом ты записывала цену",
                            //    replyMarkup: startListButton);
                            //return;
                        }

                        //Первое действие

                        else if (message.Text == "Цена указана за одно украшение")
                        {
                            //var removeKeyboard = new ReplyKeyboardRemove();
                            await botClient.SendTextMessageAsync(chatId, "Читож, ты выбрала, что цена указана за одно украшение.\nКидай мне свой список.", replyMarkup: _removeKeyboard);
                            _typeOfCount = "price for one";
                            return;

                        }

                        else if (message.Text == "Цена уже умножена")
                        {
                            //var removeKeyboard = new ReplyKeyboardRemove();
                            await botClient.SendTextMessageAsync(chatId, "Читож, ты выбрала, что цена уже умножена.\nКидай мне свой список.", replyMarkup: _removeKeyboard);
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

                        if (_typeOfCount == "price during job")
                        {
                            PriceDuringJob(botClient, update, cancellationToken);

                            //await botClient.SendTextMessageAsync(
                            //        chat.Id,
                            //        $"Сейчас у тебя {_totalSumm} деньжат",
                            //        replyMarkup: null);
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

        private static async Task PriceDuringJob(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var messageList1 = update.Message;

            if (messageList1.Text == "Закончить")
            {
                _typeOfCount = null;

                foreach (string str in _finalList)
                {
                    _messageText = string.Join("\n", _finalList);
                }
                await botClient.SendTextMessageAsync(messageList1.Chat, $"Вот твой список украшений", cancellationToken: cancellationToken);
                await botClient.SendTextMessageAsync(messageList1.Chat, $"{_messageText}", cancellationToken: cancellationToken);
                await SendSummary(botClient, messageList1.Chat.Id, _totalSumm, cancellationToken);
                _messageText = null;
                _finalList = new List<string>();
                return;
            }

            if (messageList1.Text == "Удалить прошлое украшение")
            {
                _totalSumm -= _totalSumm - _lastSumm;
                if (_finalList.Count > 0)
                {
                    _finalList.RemoveAt(_finalList.Count - 1);
                }
                await botClient.SendTextMessageAsync(messageList1.Chat, $"Удалил прошлое украшение, теперь у тебя {_totalSumm} рублей", cancellationToken: cancellationToken);
                return;
            }

            _lastSumm = _totalSumm;

            var secMessage = messageList1.Text.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var messageList = new List<string>();


            foreach (string str in secMessage)
            {
                string cleanedStr = Regex.Replace(str, @"\s+", " ").Trim();

                messageList.Add(cleanedStr);
                _finalList.Add(cleanedStr);
                //AddToList(cleanedStr);
                //_finalList.Add(Regex.Replace(str, @"\s+", " ").Trim());
                //int lastSpaceIndex = cleanExistingEntry.LastIndexOf(" "); 
                //string cleanName = cleanExistingEntry.Substring(0, lastSpaceIndex); Если Марине не надо будет в списке умножение

            }

            foreach (string str in messageList)
            {
                string[] words = str.Split(' ');
                int lastElement = words.Length - 1; //ну либо тут
                int secondLastElement = words.Length - 2; //Тут проблема с выходом индекса из массива
                var index = words[secondLastElement].Substring(1);

                bool resultCount = int.TryParse(index, out var correctIndex);

                bool resultPrice = int.TryParse(words[lastElement], out var correctPrice);
                if (resultPrice == false)
                {
                    await botClient.SendTextMessageAsync(messageList1.Chat, "Ошибочка, проверь входные данные", cancellationToken: cancellationToken);
                    return;
                }
                if (resultCount == true)
                    _totalSumm += Int32.Parse(words[lastElement]) * correctIndex;

                else
                    _totalSumm += Int32.Parse(words[lastElement]) * 1;

                if (_lastSumm == _totalSumm)
                {
                    await botClient.SendTextMessageAsync(messageList1.Chat, "Ошибочка, проверь входные данные", cancellationToken: cancellationToken);
                    return;
                }

                if (messageList1.Text != "Закончить")
                {
                    await botClient.SendTextMessageAsync(messageList1.Chat, $"Сейчас у тебя {_totalSumm} деньжат", replyMarkup: null);
                }
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
                var index = words[secondLastElement].Substring(1);

                bool resultCount = int.TryParse(index, out var correctIndex);

                bool resultPrice = int.TryParse(words[lastElement], out var correctPrice);
                if (resultPrice == false)
                {
                    await botClient.SendTextMessageAsync(messageList1.Chat, "Ошибочка, проверь входные данные", cancellationToken: cancellationToken);
                    return;
                }
                if (resultCount == true)
                    summ += Int32.Parse(words[lastElement]) * correctIndex;

                else
                    summ += Int32.Parse(words[lastElement]) * 1;
            }
            await SendSummary(botClient, messageList1.Chat.Id, summ, cancellationToken);
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
            await SendSummary(botClient, messageList1.Chat.Id, summ, cancellationToken);
        }

        //private static void AddToList(string newEntry)
        //{
        //    string pattern = @"(\d+)$"; // Поиск последнего числа в строке
        //    Match match = Regex.Match(newEntry, pattern);

        //    if (!match.Success)
        //    {
        //        Console.WriteLine("Ошибка: строка должна содержать число в конце.");
        //        return;
        //    }

        //    int newValue = int.Parse(match.Value); // Последнее число в строке

        //    for (int i = 0; i < _finalList.Count; i++)
        //    {
        //        string existingEntry = _finalList[i];

        //        // Убираем "xN" и число в конце, чтобы сравнить основные части строки
        //        string cleanExistingEntry = Regex.Replace(existingEntry, @" x\d+ \d+$", "");

        //        if (cleanExistingEntry == newEntry)/*.Substring(0, newEntry.LastIndexOf(" ")))*/ // Сравниваем без числа
        //        {
        //            // Определяем текущий множитель (если есть)
        //            Match countMatch = Regex.Match(existingEntry, @" x(\d+)");
        //            int count = countMatch.Success ? int.Parse(countMatch.Groups[1].Value) + 1 : 2;

        //            // Умножаем последнее число
        //            int updatedValue = newValue * count;


        //            int lastSpaceIndex = cleanExistingEntry.LastIndexOf(" ");
        //            string cleanName = cleanExistingEntry.Substring(0, lastSpaceIndex);


        //            // Обновляем запись
        //            _finalList[i] = $"{cleanName} x{count} {updatedValue}";
        //            return;
        //        }
        //    }

        //    // Если такой строки нет, добавляем в список
        //    _finalList.Add(newEntry);
        //}


        private static async Task SendSummary(ITelegramBotClient botclient, long chatid, double total, CancellationToken cancellationtoken)
        {
            if (total == 0)
            {
                await botclient.SendTextMessageAsync(chatid, "ошибка в данных", cancellationToken: cancellationtoken);
                return;
            }
            string response;
            if (total <= 2000)
                response = "Угощу тебя чем-нибудь, или где-нибудь";
            else if(total <= 2250)
                response = "Все мы виноваты в этом пиздеце, весь мунсоул, помазался этим говном\n(это правда сказал Тинькоффф)";
            else if(total <= 2500)
                response = "Совсем менеджеры не работают";
            else if(total <= 2750)
                response = "Не переживай, на неделе отыграешься";
            else if (total <= 3000)
                response = "Кори пукнул и денюшки улетели";
            else if (total <= 3250)
                response = "Ты молодец, муа";
            else if (total <= 3500)
                response = "Купи себе бамблби кофи";
            else if (total <= 3750)
                response = "Фььььььььььььь";
            else if (total <= 4000)
                response = "Это уже не тихо и стандартно, это праздник какой-то";
            else if (total <= 4250)
                response = "Ай, какая ты молодец";
            else if (total <= 4500)
                response = "ОГО, вот это я понимаю";
            else if (total <= 4750)
                response = "Ещё чуть чуть и прямо в рай\nИ жизнь удалась";
            else if (total <= 5000)
                response = "УРА! Этот день стал Легендарным";
            else if (total <= 6000)
                response = "Отрыв фляги...";
            else
                response = "Чета не понял";

            await botclient.SendTextMessageAsync(chatid, $"Сегодня ты заработала {total}р. \n{response}", cancellationToken: cancellationtoken);

            var restartkeyboard = new ReplyKeyboardMarkup(new[] { new KeyboardButton("Посчитать зарплату снова") })
            {
                ResizeKeyboard = true
            };
            await botclient.SendTextMessageAsync(chatid, "считаем зепку по новой?", replyMarkup: restartkeyboard);
        }
    }
}

