const axios = require('axios');
const express = require('express');
const app = express();

app.use(express.json());

app.post('/login', async (req, res) => {
  const { email, password, captchaToken } = req.body;

  // 1. Verify CAPTCHA first
  try {
    const captchaResponse = await axios.post(
      'https://www.google.com/recaptcha/api/siteverify',
      new URLSearchParams({
        secret: 'YOUR_SECRET_KEY',  // From reCAPTCHA admin
        response: captchaToken
      }),
      { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } }
    );

    if (!captchaResponse.data.success) {
      return res.status(400).json({
        message: 'CAPTCHA verification failed',
        errors: captchaResponse.data['error-codes']
      });
    }

    // 2. Only proceed if CAPTCHA is valid
    // ... your normal authentication logic ...
    return res.json({ success: true });

  } catch (error) {
    console.error('CAPTCHA verification error:', error);
    return res.status(500).json({ message: 'Internal server error' });
  }
});