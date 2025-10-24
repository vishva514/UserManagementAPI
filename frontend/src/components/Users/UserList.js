import React, { useEffect, useState } from 'react';
import { Button, Table, TableBody, TableCell, TableHead, TableRow, Typography, Box } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import api from '../../api/api';

const UserList = () => {
  const [users, setUsers] = useState([]);
  const navigate = useNavigate();

  const fetchUsers = async () => {
    try {
      const response = await api.get('/users');
      setUsers(response.data);
    } catch (error) {
      alert('Failed to fetch users');
    }
  };

  useEffect(() => {
    fetchUsers();
  }, []);

  const handleDelete = async (email) => {
    if (!window.confirm('Are you sure you want to delete this user?')) return;
    try {
      // Log for debugging
      console.log('Deleting user with email:', email);
      await api.delete(`/users/${email}`);
      fetchUsers();
    } catch (error) {
      alert('Failed to delete user');
    }
  };

  return (
    <Box>
      <Typography variant="h4" mb={2}>User List</Typography>
      <Button variant="contained" color="primary" onClick={() => navigate('/users/add')}>
        Add User
      </Button>
      <Table>
        <TableHead>
          <TableRow>
            <TableCell>Name</TableCell>
            <TableCell>Email</TableCell>
            <TableCell>Date of Birth</TableCell>
            <TableCell>Designation</TableCell>
            <TableCell>Actions</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {users.map((u) => (
            <TableRow key={u.email}>
              <TableCell>{u.name}</TableCell>
              <TableCell>{u.email}</TableCell>
              <TableCell>{u.dateOfBirth}</TableCell>
              <TableCell>{u.designation}</TableCell>
              <TableCell>
                <Button size="small" onClick={() => navigate(`/users/edit/${u.email}`)}>Edit</Button>
                <Button size="small" color="error" onClick={() => handleDelete(u.email)}>
                  Delete
                </Button>
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </Box>
  );
};

export default UserList;
