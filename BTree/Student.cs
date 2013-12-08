using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BTree
{
    public class Student
    {
        public string name;
        public string secondName;
        public BirthDay birthday;
        public Address address;
        public BirthAddress birthAddress;
        public Student()
        {
            name = GenerateName();
            secondName = GenerateLastName();
            birthday = GenerateBirthDay();
            address = GenerateAddress();
            birthAddress = GenerateBirthAddress();
        }

        public string Print()
        {
            string temp = "";
            temp = name + " " + secondName + " " + birthday.Print() + " " + birthAddress.Print();
            return temp;
        }

        string GenerateName()
        {
            string[] names = {"Vasiliy", "Alex", "Yuriy", "Nikolay", "Semen"};
            Random random = new Random();
            return names[random.Next(0, 4)];
        }

        string GenerateLastName()
        {
            string[] lastName = {"Kiselev", "Visotskiy", "Faberge", "Baranov", "Kostichev"};
            Random random = new Random();
            return lastName[random.Next(0, 4)];
        }

        BirthDay GenerateBirthDay()
        {
            BirthDay _birthDay = new BirthDay();
            return _birthDay;
        }

        Address GenerateAddress()
        {
            Address _address = new Address();
            return _address;
        }

        BirthAddress GenerateBirthAddress()
        {
            BirthAddress _birthAddress = new BirthAddress();
            return _birthAddress;
        }


    }


    public class BirthDay
    {
        public int day;
        public int month;
        public int year;

        public BirthDay()
        {
            int _day =0, _month =0, _year=0;
            GenerateDate(ref _day,ref _month,ref _year);
            day = _day;
            month = _month;
            year = _year;
        }

        private void GenerateDate(ref int _d, ref int _m, ref int _y)
        {
            Random random = new Random();
            _d = random.Next(1, 28);
            _m = random.Next(1, 12);
            _y = random.Next(1988, 1996);
        }

        public string Print()
        {
            string temp = "";
            temp = Convert.ToString(day) + " " + Convert.ToString(month) + " " + Convert.ToString(year);
            return temp;
        }
    }

    public class Address
    {
        public string city;
        public string street;
        public int homenumber;

        public Address()
        {
            string _city = "", _street = "";
            int _homenumber=0;

            GenerateAddress(ref _city,ref _street,ref _homenumber);
            city = _city;
            street = _street;
            homenumber = _homenumber;
        }

        private void GenerateAddress(ref string c, ref string s, ref int h)
        {
            string[] cities = {"Novosibirsk","Tokio","Moscow","Ugande","London"};
            string[] streets = {"Kosmonavtov", "Persidskaya", "Dolgonosova", "Lenina", "Saturn"};
            Random random = new Random();

            c = cities[random.Next(1, 4)];
            s = streets[random.Next(1, 4)];
            h = random.Next(1, 200);
        }
        public string Print()
        {
            string temp = "";
            temp = city + " " + street + " " + Convert.ToString(homenumber);
            return temp;
        }
    }

    public class BirthAddress
    {
        public string city;
        public string bornPlace;

        public BirthAddress()
        {
            string _city="", _bornPlace="";

            GenerateBirthAddress(ref _city, ref _bornPlace);
            city = _city;
            bornPlace = _bornPlace;
        }

        private void GenerateBirthAddress(ref string c, ref string b)
        {
            string[] cities = {"Ulan-Ude", "Khabarovsk", "Altan", "New-York", "Chernogorsk"};
            string[] bornPlaces = {"#1", "#2", "#3", "#4", "#5"};
            Random random = new Random();

            c = cities[random.Next(0, 4)];
            b = bornPlaces[random.Next(0, 4)];
        }
        public string Print()
        {
            string temp = "";
            temp = city + " " + bornPlace;
            return temp;
        }
    }
}
