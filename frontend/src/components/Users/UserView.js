import React from 'react';
import { Box, Typography } from '@mui/material';

const UserView = ({ user }) => {
  return (
    <Box maxWidth={400} mx="auto" mt={5}>
      <Typography variant="h5" mb={2}>User Profile</Typography>
      <Typography><strong>Name:</strong> {user.name}</Typography>
      <Typography><strong>Email:</strong> {user.email}</Typography>
      <Typography><strong>Date of Birth:</strong> {new Date(user.dateOfBirth).toLocaleDateString()}</Typography>
      <Typography><strong>Designation:</strong> {user.designation}</Typography>
    </Box>
  );
};

export default UserView;
