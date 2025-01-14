import asyncio
from aiogram import Bot, Dispatcher, types
from aiogram.filters import Command
from aiogram.types import Message
from utils import load_json

TOKEN = load_json('config.json')['bot_token']
bot = Bot(token=TOKEN)
dp = Dispatcher()

@dp.message(Command(commands=['start']))
async def process_start_command(message: Message):
    await message.reply("Привет!\nНапиши мне что-нибудь!")

@dp.message(Command(commands=['help']))
async def process_help_command(message: Message):
    await message.reply("Напиши мне что-нибудь, и я отправлю этот текст тебе в ответ!")

@dp.message()
async def echo_message(msg: Message):
    await bot.send_message(msg.from_user.id, msg.text)

async def main():
    await dp.start_polling(bot)

if __name__ == '__main__':
    asyncio.run(main())