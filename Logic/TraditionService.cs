using System;
using System.Collections.Generic;
using System.Linq;
using TravelBook.DataTraditions;
using TravelBook.Models;

namespace TravelBook.Logic
{
    public class TraditionService
    {
        // 1. Отримати всі традиції з бази
        public List<Tradition> GetAllTraditions()
        {
            using (var db = new TravelBookDbContext())
            {
                return db.Traditions.ToList();
            }
        }

        // 2. Отримати традиції конкретної країни та міста/села
        public List<Tradition> GetTraditions(string country, string? cityOrVillage)
        {
            using (var db = new TravelBookDbContext())
            {
                var query = db.Traditions.Where(t => t.Country.ToLower() == country.ToLower());

                if (!string.IsNullOrWhiteSpace(cityOrVillage))
                {
                    string normalizedCity = cityOrVillage.Trim().ToLower();
                    query = query.Where(t => t.CityOrVillage != null && t.CityOrVillage.ToLower() == normalizedCity);
                }
                else
                {
                    // Якщо місто/село не вказано, беремо тільки загальнонаціональні традиції
                    query = query.Where(t => string.IsNullOrEmpty(t.CityOrVillage));
                }

                return query.ToList();
            }
        }

        // 3. Додати нову традицію в базу даних
        public void AddTradition(Tradition tradition)
        {
            if (tradition == null) return;

            using (var db = new TravelBookDbContext())
            {
                db.Traditions.Add(tradition);
                db.SaveChanges();
            }
        }

        // 4. Оновити існуючу традицію
        public void UpdateTradition(Tradition tradition)
        {
            if (tradition == null) return;

            using (var db = new TravelBookDbContext())
            {
                var item = db.Traditions.FirstOrDefault(t => t.Id == tradition.Id);
                if (item != null)
                {
                    item.Title = tradition.Title;
                    item.Country = tradition.Country;
                    item.CityOrVillage = tradition.CityOrVillage;
                    item.Category = tradition.Category;
                    item.Description = tradition.Description;
                    item.ImagePath = tradition.ImagePath; // Оновлює шлях до фото

                    db.SaveChanges();
                }
            }
        }

        // 5. Видалити традицію за її Id
        public void DeleteTradition(int id)
        {
            using (var db = new TravelBookDbContext())
            {
                var item = db.Traditions.FirstOrDefault(t => t.Id == id);
                if (item != null)
                {
                    db.Traditions.Remove(item);
                    db.SaveChanges();
                }
            }
        }
    }
}