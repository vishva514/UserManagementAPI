import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { MemoryRouter, Routes, Route } from 'react-router-dom';
import UserForm from './UserForm';
import api from '../../api/api';

jest.mock('../../api/api');

describe('UserForm Component', () => {
  beforeEach(() => jest.clearAllMocks());

  test('renders user form for adding a user', () => {
    render(
      <MemoryRouter initialEntries={['/users/add']}>
        <Routes>
          <Route path="/users/add" element={<UserForm />} />
        </Routes>
      </MemoryRouter>
    );

    expect(screen.getByLabelText(/name/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/email/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/date of birth/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/designation/i)).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /create/i })).toBeInTheDocument();
  });

  test('submits new user data and calls API POST', async () => {
    api.post.mockResolvedValueOnce({});

    render(
      <MemoryRouter initialEntries={['/users/add']}>
        <Routes>
          <Route path="/users/add" element={<UserForm />} />
        </Routes>
      </MemoryRouter>
    );

    fireEvent.change(screen.getByLabelText(/name/i), { target: { value: 'Charlie' } });
    fireEvent.change(screen.getByLabelText(/email/i), { target: { value: 'charlie@example.com' } });
    fireEvent.change(screen.getByLabelText(/date of birth/i), { target: { value: '1990-01-01' } });
    fireEvent.change(screen.getByLabelText(/designation/i), { target: { value: 'Teacher' } });

    fireEvent.click(screen.getByRole('button', { name: /create/i }));

    await waitFor(() =>
      expect(api.post).toHaveBeenCalledWith('/users/register', expect.objectContaining({
        email: 'charlie@example.com',
        name: 'Charlie',
      }))
    );
  });
});
