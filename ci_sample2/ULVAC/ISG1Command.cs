using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace STECCommon.Equipment.ULVAC
{
    public class ISG1Command
    {
        /// <summary>
        /// ヘッダー
        /// </summary>
        static char HEADER = (char)':';

        /// <summary>
        /// アドレス
        /// </summary>
        public int Address { get; private set; }

        /// <summary>
        /// パラメータ
        /// </summary>
        public string Parameter { get; private set; }

        /// <summary>
        /// 送信用コマンドを作成する
        /// </summary>
        /// <param name="address"></param>
        /// <param name="parameter"></param>
        public ISG1Command(int address, string parameter)
        {
            Address = address;
            Parameter = parameter;
        }

        /// <summary>
        /// 受信したコマンドを解釈する
        /// チェックサムが異なる場合、例外が発生する
        /// </summary>
        /// <param name="rawResponse"></param>
        public ISG1Command(string rawResponse)
        {
            string endTrimmedResponse = rawResponse.TrimEnd();

            //  コマンド長を確認する
            //  :ADxCCの6文字はないとおかしい
            if(endTrimmedResponse.Length < 6)
            {
                throw new InvalidOperationException($"ISG1 command too short ({endTrimmedResponse})");
            }

            //  アドレスを納める
            string addressStr = endTrimmedResponse.Substring(1, 2);
            int address = 0;
            bool parseOk = int.TryParse(addressStr, out address);
            if(parseOk == false)
            {
                throw new InvalidOperationException($"ISG1 command address parse error ({addressStr})");
            }
            Address = address;

            //  パラメータを納める
            Parameter = endTrimmedResponse.Substring(3, endTrimmedResponse.Length - 3 - 2);

            //  受信したコマンドのチェックサムを確認する
            string responseCheckSum = endTrimmedResponse.Substring(rawResponse.Length - 2, 2);
            string calcedCheckSum = CalcChecksum(endTrimmedResponse.Substring(0, rawResponse.Length - 2));

            if(responseCheckSum != calcedCheckSum)
            {
                throw new InvalidOperationException($"ISG1 command checksum error");
            }

            //  送信したコマンドのチェックサムに問題が合った場合、nが返される
            if(Parameter == "n")
            {
                throw new InvalidOperationException("ISG1 command checksum error response \"n\"");
            }
        }

        /// <summary>
        /// 送受信用の文字列を返す
        /// </summary>
        /// <returns></returns>
        public string GetCommandString()
        {
            StringBuilder commandString = new StringBuilder();

            commandString.Append(HEADER);
            commandString.Append(Address.ToString("00"));
            commandString.Append(Parameter);
            commandString.Append(CalcChecksum(commandString.ToString()));

            return commandString.ToString();
        }

        /// <summary>
        /// チェックサムの文字列を算出する
        /// </summary>
        /// <param name="headerAddressParameter"></param>
        /// <returns></returns>
        public static string CalcChecksum(string headerAddressParameter)
        {
            //  ヘッダーの:はチェックサム計算に含めない
            string addressParameter = headerAddressParameter.Remove(0, 1);

            byte checkSum = (byte)addressParameter[0];

            foreach (var c in addressParameter.Skip(1))
            {
                checkSum = (byte)(checkSum ^ (byte)c);
            }

            return checkSum.ToString("X2");
        }
    }
}
