import styled from 'styled-components';
import Error500img from '/img/Error500img.png';

export function Error500() {
  return (
    <StyledWrapper className="image">
      <img src={Error500img} alt="Изображение" />
    </StyledWrapper>
  );
}
const StyledWrapper = styled.div`
  width: 55%;
  height: auto;
  margin: 0 auto;
  .image img {
    width: 100%;
    height: 100%;
    object-fit: cover;
  }
`;
