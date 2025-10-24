import React, { createContext, useState, useEffect } from 'react';
import { jwtDecode } from 'jwt-decode';


export const AuthContext = createContext();

const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);

  useEffect(() => {
    const token = localStorage.getItem('token');
    if(token) {
      const decoded = jwtDecode(token);
      setUser({ email: decoded.sub, role: decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'], name: decoded.name });
    }
  }, []);

  const login = (token) => {
    localStorage.setItem('token', token);
    const decoded = jwtDecode(token);
    setUser({ email: decoded.sub, role: decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'], name: decoded.name });
  };

  const logout = () => {
    localStorage.removeItem('token');
    setUser(null);
  };

  return (
    <AuthContext.Provider value={{ user, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

export default AuthProvider;
