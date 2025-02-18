import styled from 'styled-components';
import ErrorNotAdminImg from '/img/ErrorNotAdmin.png';
import { useNavigate } from 'react-router-dom';
import Button from '../../Components/Button/Button';

export function ErrorNotAdmin() {
  const navigate = useNavigate();
  const returnToHome = () => {
    navigate('/');
  };
  return (
    <StyledWrapper className="image">
      <img src={ErrorNotAdminImg} alt="Изображение" />
      <Button onClick={returnToHome}>На главную</Button>
    </StyledWrapper>
  );
}
const StyledWrapper = styled.div`
  width: 100%;
  height: 90vh;
  margin-right: -60px;
  margin-bottom: -30px;
  .image img {
    width: 100%;
    height: 100%;
    object-fit: cover;
  }
  Button {
    position: absolute !important;
    bottom: 100px !important;
    left: 100px !important;
    z-index: 10 !important;
  }
`;
