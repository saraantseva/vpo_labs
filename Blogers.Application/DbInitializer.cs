using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blogers.Application
{
    public static class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();

            await unitOfWork.DeleteDataBaseAsync();
            await unitOfWork.CreateDataBaseAsync();


            var p1 = new Person("Stas", DateTime.Parse("28-08-2006"));
            var b1 = new Bloger(p1, "Stasnames", 31);
            b1.Id = 1;


            var p2 = new Person("Albina", DateTime.Parse("04-04-2004"));
            var b2 = new Bloger(p2, "porphyriacus", 1000000);
            b2.Id = 2;

           
            var p3 = new Person("Alex", DateTime.Parse("15-03-1995"));
            var b3 = new Bloger(p3, "alex_gamer", 5000);
            b3.Id = 3;

            
            var p4 = new Person("Maria", DateTime.Parse("22-11-1998"));
            var b4 = new Bloger(p4, "maria_kitchen", 12000);
            b4.Id = 4;

            
            var p5 = new Person("Dima", DateTime.Parse("10-07-2000"));
            var b5 = new Bloger(p5, "dima_fitness", 8000);
            b5.Id = 5;



            await unitOfWork.BlogerRepository.AddAsync(b1);
            await unitOfWork.BlogerRepository.AddAsync(b2);
            await unitOfWork.BlogerRepository.AddAsync(b3);
            await unitOfWork.BlogerRepository.AddAsync(b4);
            await unitOfWork.BlogerRepository.AddAsync(b5);

            await unitOfWork.SaveAllAsync();

            var s1 = new Story("24 часа челлендж: делаю лабы в бугор", 5, 25, 1);
            var s2 = new Story("Почему сладкое вредно", 5, 25, 1);
            var s3 = new Story("Мяумяумяу", 5, 25, 2);
            var s4 = new Story("Ночь матанализа челлендж", 10000, 12000, 1);
            var s5 = new Story("Учимся хамить красиво", 9, 25, 1);
            var s6 = new Story("Надоело придумывать", 19, 25, 1);

            var s7 = new Story("Как я прошел Dark Souls", 120, 5000, 3);
            var s8 = new Story("Обзор новой видеокарты", 5, 1200, 3);
            var s9 = new Story("Стрим с подписчиками", 9, 3000, 3);
            var s10 = new Story("Почему киберспорт — это спорт", 230, 8000, 3);
            var s11 = new Story("Топ 5 игр 2025 года", 340, 15000, 3);

            var s12 = new Story("Как испечь идеальный хлеб", 6, 8000, 4);
            var s13 = new Story("Рецепт торта за 30 минут", 340, 5000, 4);
            var s14 = new Story("Чем заменить сахар в выпечке", 10, 3000, 4);
            var s15 = new Story("Обзор кухонной техники", 8, 2000, 4);
            var s16 = new Story("Секреты идеального кофе", 450, 7000, 4);
            var s17 = new Story("Как украсить торт в домашних условиях", 210, 4000, 4);

            var s18 = new Story("10 упражнений для пресса", 670, 12000, 5);
            var s19 = new Story("Как набрать массу без химии", 340, 6000, 5);
            var s20 = new Story("Спортивное питание: нужно или нет", 4, 8000, 5);
            var s21 = new Story("Моя система тренировок", 230, 5000, 5);
            var s22 = new Story("Как не бросить спорт после месяца", 890, 15000, 5);

            var s23 = new Story("Как продвигать блог в 2025", 45, 6000, 2);
            var s24 = new Story("Секреты съемки видео на телефон", 230, 3500, 2);
            var s25 = new Story("Как набрать миллион подписчиков", 890, 12000, 2);
            var s26 = new Story("Мои ошибки в начале карьеры", 3, 5000, 2);

            await unitOfWork.StoryRepository.AddAsync(s1);
            await unitOfWork.StoryRepository.AddAsync(s2);
            await unitOfWork.StoryRepository.AddAsync(s3);
            await unitOfWork.StoryRepository.AddAsync(s4);
            await unitOfWork.StoryRepository.AddAsync(s5);
            await unitOfWork.StoryRepository.AddAsync(s6);

            await unitOfWork.StoryRepository.AddAsync(s7);
            await unitOfWork.StoryRepository.AddAsync(s8);
            await unitOfWork.StoryRepository.AddAsync(s9);
            await unitOfWork.StoryRepository.AddAsync(s10);
            await unitOfWork.StoryRepository.AddAsync(s11);

            await unitOfWork.StoryRepository.AddAsync(s12);
            await unitOfWork.StoryRepository.AddAsync(s13);
            await unitOfWork.StoryRepository.AddAsync(s14);
            await unitOfWork.StoryRepository.AddAsync(s15);
            await unitOfWork.StoryRepository.AddAsync(s16);
            await unitOfWork.StoryRepository.AddAsync(s17);

            await unitOfWork.StoryRepository.AddAsync(s18);
            await unitOfWork.StoryRepository.AddAsync(s19);
            await unitOfWork.StoryRepository.AddAsync(s20);
            await unitOfWork.StoryRepository.AddAsync(s21);
            await unitOfWork.StoryRepository.AddAsync(s22);

            await unitOfWork.StoryRepository.AddAsync(s23);
            await unitOfWork.StoryRepository.AddAsync(s24);
            await unitOfWork.StoryRepository.AddAsync(s25);
            await unitOfWork.StoryRepository.AddAsync(s26);

            await unitOfWork.SaveAllAsync();


        }
    }
}
