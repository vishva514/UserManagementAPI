import React from 'react';
import { Routes, Route,Navigate } from 'react-router-dom';
import AuthProvider from './context/AuthContext';
import Login from './components/Auth/Login';
import Register from './components/Auth/Register';
import UserList from './components/Users/UserList';
import UserForm from './components/Users/UserForm';
import PrivateRoute from './components/Layout/PrivateRoute';

function App() {
  return (
    <AuthProvider>
      <Routes>
         <Route path="/" element={<Navigate to="/login" replace />} />
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />

        <Route
          path="/users"
          element={
            <PrivateRoute roles={['Teacher', 'Student']}>
              <UserList />
            </PrivateRoute>
          }
        />
        <Route
          path="/users/add"
          element={
            <PrivateRoute roles={['Teacher']}>
              <UserForm />
            </PrivateRoute>
          }
        />
        <Route
          path="/users/edit/:email"
          element={
            <PrivateRoute roles={['Teacher']}>
              <UserForm />
            </PrivateRoute>
          }
        />

        {/* Optional: Unauthorized Page */}
        {/* <Route path="/unauthorized" element={<Unauthorized />} /> */}
      </Routes>
    </AuthProvider>
  );
}

export default App;
