import React, { useEffect } from 'react';
import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import AuthProvider, { AuthContext } from './AuthContext';

const validToken =
  'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.' +
  'eyJzdWIiOiJ1c2VyQGV4YW1wbGUuY29tIiwibmFtZSI6IlRlc3QgVXNlciIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IlVzZXIiLCJpYXQiOjE2MzM2NDYxNzZ9.' +
  'fake-signature';

describe('AuthContext', () => {
  test('provides default values (user is null)', () => {
    let contextValue;

    const TestComponent = () => {
      contextValue = React.useContext(AuthContext);
      return null;
    };

    render(
      <AuthProvider>
        <TestComponent />
      </AuthProvider>
    );

    expect(contextValue.user).toBeNull();
    expect(typeof contextValue.login).toBe('function');
    expect(typeof contextValue.logout).toBe('function');
  });

  test('login sets user and stores token in localStorage', async () => {
    let contextValue;
    const onChange = jest.fn();

    const TestComponent = () => {
      contextValue = React.useContext(AuthContext);

      useEffect(() => {
        onChange(contextValue);
      }, [contextValue]);

      return <button onClick={() => contextValue.login(validToken)}>Login</button>;
    };

    const setItemSpy = jest.spyOn(Storage.prototype, 'setItem');

    render(
      <AuthProvider>
        <TestComponent />
      </AuthProvider>
    );

    userEvent.click(screen.getByRole('button', { name: /login/i }));

    await waitFor(() => {
      expect(onChange).toHaveBeenCalledWith(
        expect.objectContaining({
          user: expect.objectContaining({
            email: 'user@example.com',
            role: 'User',
            name: 'Test User',
          }),
        }),
      );
      expect(setItemSpy).toHaveBeenCalledWith('token', validToken);
    });

    setItemSpy.mockRestore();
  });

  test('logout clears user and removes token from localStorage', async () => {
    let contextValue;
    const onChange = jest.fn();

    const TestComponent = () => {
      contextValue = React.useContext(AuthContext);

      useEffect(() => {
        onChange(contextValue);
      }, [contextValue]);

      return (
        <>
          <button onClick={() => contextValue.login(validToken)}>Login</button>
          <button onClick={() => contextValue.logout()}>Logout</button>
        </>
      );
    };

    const removeItemSpy = jest.spyOn(Storage.prototype, 'removeItem');

    render(
      <AuthProvider>
        <TestComponent />
      </AuthProvider>
    );

    // Login then logout sequence
    userEvent.click(screen.getByRole('button', { name: /login/i }));
    await waitFor(() =>
      expect(onChange).toHaveBeenCalledWith(
        expect.objectContaining({ user: expect.any(Object) }),
      ),
    );

    userEvent.click(screen.getByRole('button', { name: /logout/i }));
    await waitFor(() => {
      expect(onChange).toHaveBeenCalledWith(
        expect.objectContaining({ user: null }),
      );
      expect(removeItemSpy).toHaveBeenCalledWith('token');
    });

    removeItemSpy.mockRestore();
  });
});
