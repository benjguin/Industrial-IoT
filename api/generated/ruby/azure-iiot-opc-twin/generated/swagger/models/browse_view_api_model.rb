# encoding: utf-8
# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License. See License.txt in the project root for
# license information.
#
# Code generated by Microsoft (R) AutoRest Code Generator 1.0.0.0
# Changes may cause incorrect behavior and will be lost if the code is
# regenerated.

module azure.iiot.opc.twin
  module Models
    #
    # browse view model
    #
    class BrowseViewApiModel
      # @return [String] Node of the view to browse
      attr_accessor :view_id

      # @return [Integer] Browses specific version of the view.
      attr_accessor :version

      # @return [DateTime] Browses at or before this timestamp.
      attr_accessor :timestamp


      #
      # Mapper for BrowseViewApiModel class as Ruby Hash.
      # This will be used for serialization/deserialization.
      #
      def self.mapper()
        {
          client_side_validation: true,
          required: false,
          serialized_name: 'BrowseViewApiModel',
          type: {
            name: 'Composite',
            class_name: 'BrowseViewApiModel',
            model_properties: {
              view_id: {
                client_side_validation: true,
                required: true,
                serialized_name: 'viewId',
                type: {
                  name: 'String'
                }
              },
              version: {
                client_side_validation: true,
                required: false,
                serialized_name: 'version',
                type: {
                  name: 'Number'
                }
              },
              timestamp: {
                client_side_validation: true,
                required: false,
                serialized_name: 'timestamp',
                type: {
                  name: 'DateTime'
                }
              }
            }
          }
        }
      end
    end
  end
end
