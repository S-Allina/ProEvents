
import './Content.css';
import homeImg from '/img/homeImg.png';
function Content() {

 
  return (
    <div className="content">
      <div className="text">
      <h1>Добро пожаловать!</h1>
      <p>Здесь ты сможешь найти самые интересные события своего города, и не только! Мы собрали для тебя широкий спектр мероприятий, которые могут вдохновить на новые впечатления и знакомства. Будь то концерты, выставки, спортивные мероприятия или культурные фестивали — наше приложение поможет тебе оставаться в курсе всего, что происходит в твоем городе.</p>
      </div>
      <div className="image">
        <img src={homeImg} alt="Изображение" />
      </div>
    </div>
  );
}

export default Content;