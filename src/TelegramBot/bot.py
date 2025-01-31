import asyncio
import aiofiles
import tempfile
import os
from aiogram import Bot, Dispatcher
from aiogram.types import ContentType, FSInputFile
from aiogram.filters import Command
from aiogram.types import Message
from PIL import Image
from utils import load_json
from neural_network import ONNXModel


config = load_json('config.json')
TOKEN = config['bot_token']
path_to_model = config['path_to_model']

bot = Bot(token=TOKEN)
dp = Dispatcher()
model = ONNXModel(path_to_model)

@dp.message(Command(commands=['start']))
async def process_start_command(message: Message):
    await message.reply("Привет!\nОтправь мне изображение панорамы шлифа керна, а я сгенерирую разметку!")

@dp.message(Command(commands=['help']))
async def process_help_command(message: Message):
    await message.reply("Отправь мне изображение панорамы шлифа керна, а я сгенерирую разметку!")

@dp.message(lambda msg: msg.content_type in [ContentType.PHOTO, ContentType.DOCUMENT])
async def handle_image(message: Message):
    try:
        if message.content_type == ContentType.DOCUMENT:
            file_name = message.document.file_name.lower()
            if not file_name.endswith(('.png', '.jpg', '.jpeg')):
                await message.reply("Пожалуйста, отправьте файл в формате PNG или JPG.")
                return
            file_id = message.document.file_id
            file_size = message.document.file_size
        else:
            file_id = message.photo[-1].file_id
            file_size = message.photo[-1].file_size

        if file_size > 20 * 1024 * 1024:
            await message.reply("Файл слишком большой. Максимальный размер: 20 МБ.")
            return

        print(f"File ID: {file_id}")
        print(f"File Size: {file_size}")

        file_info = await bot.get_file(file_id)
        if not file_info.file_path:
            await message.reply("Не удалось получить путь к файлу. Повторите попытку.")
            return

        print(f"File Path: {file_info.file_path}")

        file_bytes = await bot.download_file(file_info.file_path)
        image = Image.open((file_bytes))
        await message.reply("Обрабатываю изображение...")
        processed_image = model.predict_image(image)

        processed_image_pil = Image.fromarray(processed_image)

        with tempfile.NamedTemporaryFile(delete=False, suffix=".jpg") as tmp_file:
            processed_image_pil.save(tmp_file, format="JPEG")
            temp_file_path = tmp_file.name

        async with aiofiles.open(temp_file_path, mode='rb') as file:
                input_file = FSInputFile(file.name)
                await bot.send_photo(
                    chat_id=message.chat.id,
                    photo=input_file,
                    caption="Обработанное изображение готово!"
                )
        if os.path.exists(temp_file_path):
            os.remove(temp_file_path)

    except Exception as e:
        await message.reply(f"Произошла ошибка на стороне сервера")


async def main():
    await dp.start_polling(bot)

if __name__ == '__main__':
    asyncio.run(main())