# encoding: utf-8
# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License. See License.txt in the project root for
# license information.
#
# Code generated by Microsoft (R) AutoRest Code Generator 1.0.0.0
# Changes may cause incorrect behavior and will be lost if the code is
# regenerated.

module azure.iiot.opc.vault
  module Models
    #
    # New key pair response
    #
    class StartNewKeyPairRequestResponseApiModel
      # @return [String] Request id
      attr_accessor :request_id


      #
      # Mapper for StartNewKeyPairRequestResponseApiModel class as Ruby Hash.
      # This will be used for serialization/deserialization.
      #
      def self.mapper()
        {
          client_side_validation: true,
          required: false,
          serialized_name: 'StartNewKeyPairRequestResponseApiModel',
          type: {
            name: 'Composite',
            class_name: 'StartNewKeyPairRequestResponseApiModel',
            model_properties: {
              request_id: {
                client_side_validation: true,
                required: true,
                serialized_name: 'requestId',
                type: {
                  name: 'String'
                }
              }
            }
          }
        }
      end
    end
  end
end
