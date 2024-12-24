const About: React.FC = () => (
    <div className="flex flex-col h-full overflow-hidden p-6 bg-gray-50 text-gray-800">
      <h1 className="text-2xl font-bold mb-4">О приложении</h1>
      <p className="text-lg leading-relaxed mb-6">
        Данное приложение предназначено для удобного управления изображениями геологических образцов. Оно позволяет
        просматривать, редактировать информацию о снимках, фильтровать данные по заданным критериям, производить обработку
        изображений и эффективно организовывать работу с изображениями в рамках научных исследований.
      </p>
      <p className="text-lg font-medium">
        По всем вопросам пишите:{" "}
        <a
          href="mailto:vitamin20021312@gmail.com"
          className="text-blue-500 hover:underline"
        >
          vitamin20021312@gmail.com
        </a>
      </p>
    </div>
  );
  
  export default About;
  