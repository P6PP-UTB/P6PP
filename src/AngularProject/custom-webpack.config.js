module.exports = {
    module: {
      rules: [
        {
          test: /.(woff|woff2|eot|ttf|svg)$/,
          type: 'asset/resource',
          generator: {
            filename: 'assets/fonts/[name][ext]'
          }
        }
      ]
    }
  };