using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modification
{
    //класс для растчета зароботных плат
    public class SalaryCalculator
    {
        //метод для рассчета базовой зп без вычетов
        public double CalculateBaseSalary(double hours, double rate)//объявляем функции которая принемает ставку и количество часов
        {
            return hours * rate;//возвращаем произведение часов на ставку 
        }
        //метод для расчета зп с учетом налога 13%
        public double CalculateNetSalary(double hours, double rate)
        {
            double gross = CalculateBaseSalary(hours, rate);//получаем и записываем в переменную зп до вычета налога
            double tax = gross * 0.13;//записываем в переменную налог 13 процентов
            return gross - tax;//возращаем чистую зп после вычета налога
        }
    }
    // модифицированный модуль добавляем премию и поэтапный налог
    public class ModifiedSalaryCalculator : SalaryCalculator //создаем новый класс и наследуемся от салари калькулятор
    {
        //переопределяем метод расчета зп с учетом новых правил
        public new double CalculateNetSalary(double hours, double rate, double bonus = 0)//добавляется еще 1 параметр премия 
        {
            double gross = CalculateBaseSalary(hours, rate);//базовая часть для расчета зп без налога 
            gross += bonus;//добавляем премию к зп до вычета налога 
            double tax = 0;
            if (gross <= 25000)
                tax = gross * 0.10;//низкий налог для маленькой зп 
            else
                tax = 25000 * 0.10 + (gross - 25000) * 0.20;//10% налога с первых 250000 + 20% с остатка 
            return gross - tax;//возвращаем обновленную чистую зп 
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            //тестирование исходного модуля 
            SalaryCalculator oldCalc = new SalaryCalculator();//создаем экземпляр класса до модификации 
            double oldNet = oldCalc.CalculateNetSalary(160, 250);//считаем зп до модификации 160ч*250р
            Console.WriteLine($"Старая версия :{oldNet}");//отображение результата в консоль 
            ModifiedSalaryCalculator newCalc=new ModifiedSalaryCalculator();//Создаем экземпляр класса
            double newNet = newCalc.CalculateNetSalary(160, 250, 3000);//Считаем зп с учетом премии
            Console.WriteLine($"Novaя версия: {newNet}");//Отоброжаем в консоль результаты модифицированного класса
            //демонстрация обратной совместимости(бкз премии)
            double noBonus = newCalc.CalculateNetSalary(160, 250);
            Console.WriteLine($"Bez premii: {noBonus}");

        }    
    }
}