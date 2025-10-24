import React, { useState, useContext } from 'react';
import { TextField, Button, Typography, Box } from '@mui/material';
import { Link } from 'react-router-dom';
import api from '../../api/api';
import { AuthContext } from '../../context/AuthContext';
import { useNavigate } from 'react-router-dom';

const Login = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const { login } = useContext(AuthContext);
  const navigate = useNavigate();
  const [error, setError] = useState('');

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      const response = await api.post('/users/login', { email, password });
      login(response.data.token);
      navigate('/users');
    } catch (err) {
      setError('Invalid Email or Password');
    }
  };

  return (
    <Box maxWidth={400} mx="auto" mt={10}>
      <Typography variant="h4" mb={3}>Login</Typography>
      <form onSubmit={handleSubmit}>
        <TextField fullWidth label="Email" value={email} onChange={e => setEmail(e.target.value)} margin="normal" required />
        <TextField fullWidth label="Password" type="password" value={password} onChange={e => setPassword(e.target.value)} margin="normal" required />
        {error && <Typography color="error">{error}</Typography>}
        <Button fullWidth variant="contained" type="submit" sx={{ mt: 2 }}>Login</Button>
      </form>
      {/* Register link below the form */}
      <Typography align="center" mt={2}>
        Don't have an account? <Link to="/register">Register here</Link>
      </Typography>
    </Box>
  );
};

export default Login;
