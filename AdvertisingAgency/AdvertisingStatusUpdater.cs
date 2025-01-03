using AdvertisingAgency;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AdvertisingAgency.Services
{
    public class AdvertisingStatusUpdater : IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private Timer _timer;

        public AdvertisingStatusUpdater(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Запускаем таймер каждый час
            _timer = new Timer(UpdateStatuses, null, TimeSpan.Zero, TimeSpan.FromHours(1));
            return Task.CompletedTask;
        }

        private void UpdateStatuses(object state)
        {
            // Создаем новый scope для работы с scoped-сервисами
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AdvertisingAgencyContext>();
                UpdateAdvertisingStatuses(context).Wait();
            }
        }

        private async Task UpdateAdvertisingStatuses(AdvertisingAgencyContext context)
        {
            var currentDate = DateTime.Now;

            // Получаем все заявки
            var advertisings = await context.Advertisings.ToListAsync();

            foreach (var advertising in advertisings)
            {
                // Проверяем, активна ли заявка
                if (advertising.DateStart <= currentDate && advertising.DateStart.AddDays(advertising.Duration) > currentDate)
                {
                    advertising.IsActive = true;
                }
                else
                {
                    advertising.IsActive = false;
                }

                // Удаляем заявку, если время истекло
                if (advertising.DateStart.AddDays(advertising.Duration) <= currentDate)
                {
                    context.Advertisings.Remove(advertising);
                }
            }

            // Сохраняем изменения в базе данных
            await context.SaveChangesAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}