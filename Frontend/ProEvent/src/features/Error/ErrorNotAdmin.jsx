import styled from 'styled-components';
import ErrorNotAdminImg from '/img/ErrorNotAdmin.png';

export function ErrorNotAdmin() {
  return (
    <StyledWrapper className="image">
      <img src={ErrorNotAdminImg} alt="Изображение" />
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
`;
