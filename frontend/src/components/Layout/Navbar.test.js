const navigateMock = jest.fn();

jest.mock('react-router-dom', () => {
  const actual = jest.requireActual('react-router-dom');
  return {
    ...actual,
    useNavigate: () => navigateMock,
  };
});

import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { MemoryRouter } from 'react-router-dom';
import AuthProvider, { AuthContext } from '../../context/AuthContext';
import Navbar from './Navbar';

beforeEach(() => {
  navigateMock.mockClear();
});

const renderWithAuth = (user = null, logout = jest.fn()) => {
  return render(
    <MemoryRouter>
      <AuthContext.Provider value={{ user, logout }}>
        <Navbar />
      </AuthContext.Provider>
    </MemoryRouter>
  );
};

describe('Navbar Component', () => {
  test('renders login and register buttons if no user', () => {
    renderWithAuth(null);

    expect(screen.getByRole('button', { name: /login/i })).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /register/i })).toBeInTheDocument();
    expect(screen.queryByRole('button', { name: /logout/i })).toBeNull();
  });

  test('renders user info and logout button if user is logged in', () => {
    const user = { name: 'John Doe', role: 'Admin' };
    renderWithAuth(user);

    expect(screen.getByText(/john doe/i)).toBeInTheDocument();
    expect(screen.getByText(/admin/i)).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /logout/i })).toBeInTheDocument();
  });

  test('calls logout and navigates to login on logout button click', () => {
    const mockLogout = jest.fn();
    const user = { name: 'John Doe', role: 'Admin' };

    renderWithAuth(user, mockLogout);

    const logoutButton = screen.getByRole('button', { name: /logout/i });
    userEvent.click(logoutButton);

    expect(mockLogout).toHaveBeenCalled();
    expect(navigateMock).toHaveBeenCalledWith('/login');
  });
});
