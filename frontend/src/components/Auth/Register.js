import React, { useState } from 'react';
import { Box, TextField, Button, Typography } from '@mui/material';
import { useNavigate } from 'react-router-dom'; // ADD THIS
import axios from '../../api/api';

const Register = () => {
  const [name, setName] = useState('');
  const [email, setEmail] = useState('');
  const [dob, setDob] = useState('');
  const [designation, setDesignation] = useState('Student');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  
  const navigate = useNavigate(); // ADD THIS

  const handleRegister = async () => {
    try {
      await axios.post('/users/register', {
        name,
        email,
        dateOfBirth: dob,
        designation,
        password
      });
      
      setSuccess('Registration successful! You can now login.');
      setError('');
      
      // ADD THIS: Auto-redirect to login after 2 seconds
      setTimeout(() => {
        navigate('/login');
      }, 2000);
      
    } catch (err) {
      setError('Registration failed. ' + (err.response?.data || err.message));
      setSuccess('');
    }
  };

  return (
    <Box maxWidth={400} mx="auto" mt={10}>
      <Typography variant="h4" mb={3}>Register</Typography>
      <TextField 
        fullWidth 
        label="Name" 
        value={name} 
        onChange={e => setName(e.target.value)} 
        margin="normal" 
        required 
      />
      <TextField 
        fullWidth 
        label="Email" 
        type="email" 
        value={email} 
        onChange={e => setEmail(e.target.value)} 
        margin="normal" 
        required 
      />
      <TextField 
        fullWidth 
        label="Date of Birth" 
        type="date" 
        value={dob} 
        onChange={e => setDob(e.target.value)} 
        margin="normal" 
        required 
        InputLabelProps={{ shrink: true }} 
      />
      <TextField 
        fullWidth 
        label="Designation" 
        select 
        value={designation} 
        onChange={e => setDesignation(e.target.value)} 
        margin="normal" 
        required 
        SelectProps={{ native: true }}
      >
        <option value="Student">Student</option>
        <option value="Teacher">Teacher</option>
      </TextField>
      <TextField 
        fullWidth 
        label="Password" 
        type="password" 
        value={password} 
        onChange={e => setPassword(e.target.value)} 
        margin="normal" 
        required 
      />
      
      {error && <Typography color="error" sx={{ mt: 2 }}>{error}</Typography>}
      {success && <Typography color="primary" sx={{ mt: 2 }}>{success}</Typography>}
      
      <Button 
        fullWidth 
        variant="contained" 
        onClick={handleRegister} 
        sx={{ mt: 2 }}
      >
        Register
      </Button>
    </Box>
  );
};

export default Register;
