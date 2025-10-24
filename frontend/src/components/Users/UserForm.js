import React, { useState, useEffect } from 'react';
import { Box, TextField, Button, Typography } from '@mui/material';
import { useNavigate, useParams } from 'react-router-dom';
import api from '../../api/api';

const UserForm = () => {
  const { email } = useParams(); // email param for edit mode
  const [user, setUser] = useState({
    name: '',
    email: '',
    dateOfBirth: '',
    designation: 'Student',
    password: '',
  });
  const [error, setError] = useState('');
  const navigate = useNavigate();
  const isEdit = Boolean(email);

  useEffect(() => {
    if (isEdit) {
      api.get(`/users`)  // Or ideally: /users/{email} endpoint if created
        .then(res => {
          const found = res.data.find(u => u.email === email);
          if (found) {
            setUser({
              name: found.name,
              email: found.email,
              dateOfBirth: found.dateOfBirth.split('T')[0],
              designation: found.designation,
              password: '',
            });
          }
        })
        .catch(err => setError('Failed to load user'));
    }
  }, [email, isEdit]);

  const handleChange = (e) => {
    setUser({...user, [e.target.name]: e.target.value});
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      if (isEdit) {
        await api.put('/users', user);
      } else {
        await api.post('/users/register', user);
      }
      navigate('/users');
    } catch (err) {
      setError('Error saving user: ' + (err.response?.data || err.message));
    }
  };

  return (
    <Box maxWidth={450} mx="auto" mt={5}>
      <Typography variant="h5" mb={3}>{isEdit ? 'Edit User' : 'Add User'}</Typography>
      <form onSubmit={handleSubmit}>
        <TextField label="Name" name="name" value={user.name} onChange={handleChange} fullWidth required margin="normal" />
        {!isEdit && (
          <TextField label="Email" name="email" value={user.email} onChange={handleChange} fullWidth required margin="normal" />
        )}
        <TextField
          label="Date of Birth"
          name="dateOfBirth"
          type="date"
          value={user.dateOfBirth}
          onChange={handleChange}
          fullWidth
          required
          margin="normal"
          InputLabelProps={{ shrink: true }}
        />
        <TextField
          label="Designation"
          name="designation"
          value={user.designation}
          onChange={handleChange}
          select
          fullWidth
          required
          margin="normal"
          SelectProps={{ native: true }}
        >
          <option value="Student">Student</option>
          <option value="Teacher">Teacher</option>
        </TextField>
        <TextField label="Password" name="password" type="password" value={user.password} onChange={handleChange} fullWidth margin="normal" required={!isEdit} />
        {error && <Typography color="error">{error}</Typography>}
        <Button variant="contained" type="submit" sx={{ mt: 2 }}>{isEdit ? 'Update' : 'Create'}</Button>
      </form>
    </Box>
  );
};

export default UserForm;
